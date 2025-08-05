using CoreVisionBAL.AppUsers;
using CoreVisionBAL.Clients;
using CoreVisionBAL.ExceptionHandler;
using CoreVisionBAL.License;
using CoreVisionBAL.Token;
using CoreVisionConfig.Configuration;
using CoreVisionDAL.Context;
using CoreVisionFoundation.Security;
using CoreVisionServiceModels.AppUser;
using CoreVisionServiceModels.Enums;
using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;
using CoreVisionServiceModels.Foundation.Base.Enums;
using CoreVisionServiceModels.Foundation.Token;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoreVisionFoundation.Controllers.Token
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ExternalUserController : ControllerBase
    {
        #region Properties
        private readonly APIConfiguration _configuration;
        private readonly JwtHandler _jwtHandler;
        private readonly TokenProcess _tokenProcess;
        private readonly ClientUserProcess _clientUserProcess;
        private readonly ClientCompanyDetailProcess _clientCompanyDetailsProcess;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ExternalUserProcess _externalUserProcess;
        private readonly ApiDbContext _apiDbContext;
        private readonly UserTestLicenseDetailsProcess _userTestLicenseDetailsProcess;
        #endregion Properties

        #region Constructor
        public ExternalUserController(APIConfiguration config, TokenProcess tokenProcess, ApiDbContext apiDbContext, 
            ClientUserProcess clientUserProcess, UserTestLicenseDetailsProcess userTestLicenseDetailsProcess, ClientCompanyDetailProcess clientCompanyDetailsProcess,
            IHttpClientFactory httpClientFactory, JwtHandler jwtHandler, IHttpContextAccessor httpContextAccessor, ExternalUserProcess googleAuthProcess)
        {
            _configuration = config;
            _jwtHandler = jwtHandler;
            _tokenProcess = tokenProcess;
            _clientUserProcess = clientUserProcess;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _externalUserProcess = googleAuthProcess;
            _apiDbContext = apiDbContext;
            _userTestLicenseDetailsProcess = userTestLicenseDetailsProcess;
            _clientCompanyDetailsProcess = clientCompanyDetailsProcess;
        }

        #endregion Constructor

        #region Additional Method

        #endregion Additional Method

        #region Login / Signup

        [HttpGet("googlelogin")]
        [HttpGet("googlesignup")]
        public async Task<ActionResult<ApiResponse<TokenResponseSM>>> GoogleLogin(string idToken)
        {
            if (string.IsNullOrEmpty(idToken))
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Id Token cannot be null or empty", "Id Token cannot be null or empty");
            }
            string companyCode = "123";
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
            if (payload == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                    $"Error in decoding Google id token, token is {idToken}",
                    "We couldn't verify your Google login. Please try again.");
            }
            if (string.IsNullOrEmpty(payload.Email))
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                   $"Google ID token is missing an email address. This may occur if the user denied email permissions or if there was an issue with token generation. Token: {idToken}",
                   "We couldn't retrieve your email from Google. Please check your Google account settings and try again.");
            }
            var existingClientUser = await _clientUserProcess.GetClientUserByEmail(payload.Email);

            string base64Picture = null;
            bool isNewLogin = false;
            if (existingClientUser != null)
            {                              

                if (existingClientUser.IsEmailConfirmed == false)
                {
                    isNewLogin = true;
                    try
                    {
                        if (string.IsNullOrEmpty(existingClientUser.ProfilePicturePath))
                        {
                            using (var httpClient = _httpClientFactory.CreateClient())
                            {
                                var imageBytes = await httpClient.GetByteArrayAsync(payload.Picture);
                                base64Picture = Convert.ToBase64String(imageBytes);
                                existingClientUser.ProfilePicturePath = base64Picture;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        existingClientUser.ProfilePicturePath = null;
                    }
                    existingClientUser = await _clientUserProcess.UpdateClientUser(existingClientUser.Id, existingClientUser, true);

                }
                var externalUser = await _externalUserProcess.GetExternalUserByClientUserIdandTypeAsync(existingClientUser.Id, ExternalUserTypeSM.Google);
                if (externalUser == null)
                {
                    var externalGoogleUser = new ExternalUserSM()
                    {
                        ClientUserId = existingClientUser.Id,
                        RefreshToken = idToken,
                        ExternalUserType = ExternalUserTypeSM.Google
                    };
                    externalUser = await _externalUserProcess.AddExternalUser(externalGoogleUser);
                }
                if (isNewLogin == true)
                {
                    var res = await _userTestLicenseDetailsProcess.AddTrialLicenseDetails(existingClientUser.Id);
                    if (res == null)
                    {
                        throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Something went wrong while adding trial license details to user with Id: {existingClientUser.Id}", "Something went wrong while adding trial license details");
                    }
                }
                if (externalUser != null) 
                    
                    return await CreateTokenForUser(existingClientUser, companyCode, isNewLogin);
                //throw new CoreVisionException(ApiErrorTypeSM.Success_NoLog, "Google Login Details Already Exist...Sign in Instead", "Google Login Details Already Exist. Please use login.");
                else // error in adding external user
                    throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                    $"Error in saving ClientUser and External Google user details, token is: {idToken}",
                    "Something went wrong in saving your details...Please Try Again");
            }
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    var imageBytes = await httpClient.GetByteArrayAsync(payload.Picture);
                    base64Picture = Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception ex)
            {
                base64Picture = null;
            }
            var newUser = new ClientUserSM()
            {
                LoginId = payload.Email,
                EmailId = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                IsEmailConfirmed = true,
                ProfilePicturePath = base64Picture,
                Gender = GenderSM.Unknown,
            };
            var externalUserType = ExternalUserTypeSM.Google;
            ClientUserSM newUserSM = await _externalUserProcess.AddClientUserandExternalUserDetails(newUser, idToken, companyCode, externalUserType);
            //await _externalUserProcess.AssignExternalUserWithCompany(newUserSM.Id, companyCode);
            if (newUserSM == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                    $"Failed to save ClientUser and External Google user details. Token: {idToken}",
                    "We encountered an issue while creating your account. Please try again.");
            }
            else
            {
                var res = await _userTestLicenseDetailsProcess.AddTrialLicenseDetails(newUserSM.Id);
                if (res == null)
                {
                    throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, $"Something went wrong while adding trial license details to user with Id: {newUserSM.Id}", "Something went wrong while adding trial license details");
                }

                return await CreateTokenForUser(newUserSM, companyCode, true);
            }
        }

        #endregion Login 

        /*#region Google Authentication

        #region Google SignUp

        [HttpGet("googlesignup")]
        public async Task<ActionResult<ApiResponse<TokenResponseSM>>> GoogleSignUp(string idToken, string companyCode)
        {
            if (string.IsNullOrEmpty(idToken))
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Id Token cannot be null or empty", "Id Token cannot be null or empty");
            }
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
            var existingClientUser = await _clientUserProcess.GetClientUserByEmail(payload.Email);
            string base64Picture;
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    var imageBytes = await httpClient.GetByteArrayAsync(payload.Picture);
                    base64Picture = Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception ex)
            {
                base64Picture = null;
            }
           
            if (existingClientUser != null)
            {
                var externalGoogleUser = new ExternalUserSM()
                {
                    ClientUserId = existingClientUser.Id,
                    RefreshToken = idToken,
                    ExternalUserType = ExternalUserTypeSM.Google
                };
                // confirm email as its from its google account
                if (!existingClientUser.IsEmailConfirmed)
                {
                    if (existingClientUser.ProfilePicturePath.IsNullOrEmpty())
                    {
                        existingClientUser.ProfilePicturePath = base64Picture;
                    }
                    existingClientUser = await _clientUserProcess.UpdateClientUser(existingClientUser.Id, existingClientUser,true);
                }
                var externalUser = await _externalUserProcess.GetExternalUserByClientUserIdandTypeAsync(existingClientUser.Id, ExternalUserTypeSM.Google);
                if (externalUser == null)
                {
                    externalUser = await _externalUserProcess.AddExternalUser(externalGoogleUser);
                }
                if (externalUser != null) // external user added / is already available
                    throw new CoreVisionException(ApiErrorTypeSM.Success_NoLog, "Google Login Details Already Exist...Sign in Instead", "Google Login Details Already Exist. Please use login.");
                else // error in adding external user
                    throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Internal error occured, try again after sometime. If problem persists, contact support.", $"Error in adding external user with id token {idToken}");
            }
           
            var newUser = new ClientUserSM()
            {
                LoginId = payload.Email,
                EmailId = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                IsEmailConfirmed = true,
                ProfilePicturePath = base64Picture,
                Gender = GenderSM.Unknown,
            };
            var externalUserType = ExternalUserTypeSM.Google;
            ClientUserSM newUserSM = await _externalUserProcess.AddClientUserandExternalUserDetails(newUser, idToken, companyCode, externalUserType);
            if (newUserSM == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Error in saving your details... Try Again", "Error in saving your details... Try Again");
            }
            else
            {
                return await CreateTokenForUser(newUserSM, companyCode);
            }
        }

        #endregion Google SignUp

        #region Login 

        [HttpGet("googlelogin")]
        public async Task<ActionResult<ApiResponse<TokenResponseSM>>> GoogleLogin(string idToken, string companyCode)
        {
            if (string.IsNullOrEmpty(idToken))
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Id Token cannot be null or empty", "Id Token cannot be null or empty");
            }
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
            var existingClientUser = await _clientUserProcess.GetClientUserByEmail(payload.Email);
            string base64Picture;
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    var imageBytes = await httpClient.GetByteArrayAsync(payload.Picture);
                    base64Picture = Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception ex)
            {
                base64Picture = null;
            }

            if (existingClientUser != null)
            {
                var externalGoogleUser = new ExternalUserSM()
                {
                    ClientUserId = existingClientUser.Id,
                    RefreshToken = idToken,
                    ExternalUserType = ExternalUserTypeSM.Google
                };
                // confirm email as its from its google account
                if (!existingClientUser.IsEmailConfirmed)
                {
                    if (existingClientUser.ProfilePicturePath.IsNullOrEmpty())
                    {
                        existingClientUser.ProfilePicturePath = base64Picture;
                    }
                    existingClientUser = await _clientUserProcess.UpdateClientUser(existingClientUser.Id, existingClientUser,true);
                }
                var externalUser = await _externalUserProcess.GetExternalUserByClientUserIdandTypeAsync(existingClientUser.Id, ExternalUserTypeSM.Google);
                if (externalUser == null)
                {
                    externalUser = await _externalUserProcess.AddExternalUser(externalGoogleUser);
                }
                if (externalUser != null) // external user added / is already available
                                          // generate token
                    return await CreateTokenForUser(existingClientUser, companyCode);
                //throw new CodeVisionException(ApiErrorTypeSM.Success_NoLog, "Google Login Details Already Exist...Sign in Instead", "Google Login Details Already Exist. Please use login.");
                else // error in adding external user
                    throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Internal error occured, try again after sometime. If problem persists, contact support.", $"Error in adding external user with id token {idToken}");
            }
            
            var newUser = new ClientUserSM()
            {
                LoginId = payload.Email,
                EmailId = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                IsEmailConfirmed = true,
                ProfilePicturePath = base64Picture,
                Gender = GenderSM.Unknown,
            };
            var externalUserType = ExternalUserTypeSM.Google;
            ClientUserSM newUserSM = await _externalUserProcess.AddClientUserandExternalUserDetails(newUser, idToken, companyCode, externalUserType);
            if (newUserSM == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Error in saving your details... Try Again", "Error in saving your details... Try Again");
            }
            else
            {
                return await CreateTokenForUser(newUserSM, companyCode);
            }
        }

        #endregion Login 

        #endregion

        #region Facebook Authentication

        #region Facebook Sign Up

        [HttpGet("facebooksignup")]
        public async Task<ActionResult<ApiResponse<TokenResponseSM>>> FacebookSignUp(string idToken, string companyCode)
        {
            if (string.IsNullOrEmpty(idToken))
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Id Token cannot be null or empty", "Id Token cannot be null or empty");
            }

            var httpClient = _httpClientFactory.CreateClient();

            var userInfoUrl = $"https://graph.facebook.com/me?fields=id,name,email,first_name,last_name,picture&access_token={idToken}";
            var userInfoResponse = await httpClient.GetStringAsync(userInfoUrl);

            using var userInfoDoc = JsonDocument.Parse(userInfoResponse);
            var root = userInfoDoc.RootElement;

            // Check if the necessary properties exist before accessing them
            string userId = root.TryGetProperty("id", out var idProperty) ? idProperty.GetString() : null;
            string emailId = root.TryGetProperty("email", out var emailProperty) ? emailProperty.GetString() : null;
            string firstName = root.TryGetProperty("first_name", out var firstNameProperty) ? firstNameProperty.GetString() : null;
            string lastName = root.TryGetProperty("last_name", out var lastNameProperty) ? lastNameProperty.GetString() : null;
            string pictureUrl = root.TryGetProperty("picture", out var pictureProperty) &&
                                pictureProperty.TryGetProperty("data", out var dataProperty) ?
                                dataProperty.GetProperty("url").GetString() : null;

            if (emailId.IsNullOrEmpty())
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                    "We couldn't retrieve your Facebook email address. Please ensure that your Facebook account is properly linked and try again.",
                    "We couldn't retrieve your Facebook email address. Please ensure that your Facebook account is properly linked and try again.");
            }

            var existingClientUser = await _clientUserProcess.GetClientUserByEmail(emailId);

            string base64Picture;
            try
            {
                using (var httpClients = _httpClientFactory.CreateClient())
                {
                    var imageBytes = await httpClient.GetByteArrayAsync(pictureUrl);
                    base64Picture = Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception ex)
            {
                base64Picture = null;
            }

            if (existingClientUser != null)
            {
                var externalGoogleUser = new ExternalUserSM()
                {
                    ClientUserId = existingClientUser.Id,
                    RefreshToken = idToken,
                    ExternalUserType = ExternalUserTypeSM.Facebook
                };
                // confirm email as its from its google account
                if (!existingClientUser.IsEmailConfirmed)
                {
                    if (existingClientUser.ProfilePicturePath.IsNullOrEmpty())
                    {
                        existingClientUser.ProfilePicturePath = base64Picture;
                    }
                    existingClientUser = await _clientUserProcess.UpdateClientUser(existingClientUser.Id, existingClientUser,true);
                }
                var externalUser = await _externalUserProcess.GetExternalUserByClientUserIdandTypeAsync(existingClientUser.Id, ExternalUserTypeSM.Google);
                if (externalUser == null)
                {
                    externalUser = await _externalUserProcess.AddExternalUser(externalGoogleUser);
                }
                if (externalUser != null) // external user added / is already available

                    throw new CoreVisionException(ApiErrorTypeSM.Success_NoLog, "Google Login Details Already Exist...Sign in Instead", "Google Login Details Already Exist. Please use login.");
                else // error in adding external user
                    throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Internal error occured, try again after sometime. If problem persists, contact support.", $"Error in adding external user with id token {idToken}");
            }
            
            var newUser = new ClientUserSM()
            {
                LoginId = emailId,
                EmailId = emailId,
                FirstName = string.IsNullOrEmpty(firstName) ? emailId.Split('@')[0] : firstName,
                LastName = lastName,
                IsEmailConfirmed = true,
                ProfilePicturePath = base64Picture,
                Gender = GenderSM.Unknown,
            };
            var externalUserType = ExternalUserTypeSM.Facebook;
            ClientUserSM newUserSM = await _externalUserProcess.AddClientUserandExternalUserDetails(newUser, idToken, companyCode, externalUserType);
            if (newUserSM == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Error in saving your details... Try Again", "Error in saving your details... Try Again");
            }
            else
            {
                return await CreateTokenForUser(newUserSM, companyCode);
            }
        }

        #endregion Facebook Sign Up

        #region Login 

        [HttpGet("facebooklogin")]
        public async Task<ActionResult<ApiResponse<TokenResponseSM>>> FacebookbLogin(string idToken, string companyCode)
        {
            if (string.IsNullOrEmpty(idToken))
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Id Token cannot be null or empty", "Id Token cannot be null or empty");
            }
            var httpClient = _httpClientFactory.CreateClient();

            var userInfoUrl = $"https://graph.facebook.com/me?fields=id,name,email,first_name,last_name,picture&access_token={idToken}";
            var userInfoResponse = await httpClient.GetStringAsync(userInfoUrl);

            using var userInfoDoc = JsonDocument.Parse(userInfoResponse);
            var root = userInfoDoc.RootElement;

            // Check if the necessary properties exist before accessing them
            string userId = root.TryGetProperty("id", out var idProperty) ? idProperty.GetString() : null;
            string emailId = root.TryGetProperty("email", out var emailProperty) ? emailProperty.GetString() : null;
            string firstName = root.TryGetProperty("first_name", out var firstNameProperty) ? firstNameProperty.GetString() : null;
            string lastName = root.TryGetProperty("last_name", out var lastNameProperty) ? lastNameProperty.GetString() : null;
            string pictureUrl = root.TryGetProperty("picture", out var pictureProperty) &&
                                pictureProperty.TryGetProperty("data", out var dataProperty) ?
                                dataProperty.GetProperty("url").GetString() : null;

            if (emailId.IsNullOrEmpty())
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log,
                    "We couldn't retrieve your Facebook email address. Please ensure that your Facebook account is properly linked and try again.",
                    "We couldn't retrieve your Facebook email address. Please ensure that your Facebook account is properly linked and try again.");
            }

            var existingClientUser = await _clientUserProcess.GetClientUserByEmail(emailId);
            string base64Picture;
            try
            {
                using (var httpClients = _httpClientFactory.CreateClient())
                {
                    var imageBytes = await httpClient.GetByteArrayAsync(pictureUrl);
                    base64Picture = Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception ex)
            {
                base64Picture = null;
            }
            if (existingClientUser != null)
            {
                var externalFbUser = new ExternalUserSM()
                {
                    ClientUserId = existingClientUser.Id,
                    RefreshToken = idToken,
                    ExternalUserType = ExternalUserTypeSM.Facebook
                };
                // confirm email as its from its google account
                if (!existingClientUser.IsEmailConfirmed)
                {
                    if (existingClientUser.ProfilePicturePath.IsNullOrEmpty())
                    {
                        existingClientUser.ProfilePicturePath = base64Picture;
                    }
                    existingClientUser = await _clientUserProcess.UpdateClientUser(existingClientUser.Id, existingClientUser, true);
                }
                var externalUser = await _externalUserProcess.GetExternalUserByClientUserIdandTypeAsync(existingClientUser.Id, ExternalUserTypeSM.Facebook);
                if (externalUser == null)
                {
                    externalUser = await _externalUserProcess.AddExternalUser(externalFbUser);
                }
                if (externalUser != null) // external user added / is already available
                                          // generate token
                    return await CreateTokenForUser(existingClientUser, companyCode);
                else // error in adding external user
                    throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Internal error occured, try again after sometime. If problem persists, contact support.", $"Error in adding external user with id token {idToken}");
            }
            
            var newUser = new ClientUserSM()
            {
                LoginId = emailId,
                EmailId = emailId,
                FirstName = string.IsNullOrEmpty(firstName) ? emailId.Split('@')[0] : firstName,
                LastName = lastName,
                IsEmailConfirmed = true,
                ProfilePicturePath = base64Picture,
                Gender = GenderSM.Unknown,
            };
            var externalUserType = ExternalUserTypeSM.Facebook;
            ClientUserSM newUserSM = await _externalUserProcess.AddClientUserandExternalUserDetails(newUser, idToken, companyCode, externalUserType);
            if (newUserSM == null)
            {
                throw new CoreVisionException(ApiErrorTypeSM.Fatal_Log, "Error in saving your details... Try Again", "Error in saving your details... Try Again");
            }
            else
            {
                return await CreateTokenForUser(newUserSM, companyCode);
            }
        }

        #endregion Login 

        #endregion

        #region Apple Authentication

        #endregion*/

        #region PrivateFunctions

        private async Task<ActionResult<ApiResponse<TokenResponseSM>>> CreateTokenForUser(ClientUserSM clientUserSM, string companyCode, bool isNewLogin = false)
        {
            if (clientUserSM.LoginStatus == LoginStatusSM.Disabled)
            {
                return Unauthorized(ModelConverter.FormNewErrorResponse(DomainConstants.DisplayMessages.Display_UserDisabled, ApiErrorTypeSM.Access_Denied_Log));
            }
            ICollection<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,clientUserSM.LoginId),
                new Claim(ClaimTypes.Role,clientUserSM.RoleType.ToString()),
                new Claim(ClaimTypes.GivenName,$"{clientUserSM.FirstName} {clientUserSM.LastName}" ),
                new Claim(ClaimTypes.Email,clientUserSM.EmailId),
                new Claim(DomainConstants.ClaimsRoot.Claim_DbRecordId,clientUserSM.Id.ToString())
            };
            if (!string.IsNullOrWhiteSpace(companyCode))
            {
                var companyDetails = await _clientCompanyDetailsProcess.GetClientCompanyByCompanyCode(companyCode);
                if (companyDetails != null)
                {
                    clientUserSM.ClientCompanyDetailId = companyDetails.Id;
                }
                claims.Add(new Claim(DomainConstants.ClaimsRoot.Claim_ClientCode, companyCode));
                claims.Add(new Claim(DomainConstants.ClaimsRoot.Claim_ClientId, clientUserSM.ClientCompanyDetailId.ToString()));
            }
            var expiryDate = DateTime.Now.AddDays(_configuration.DefaultTokenValidityDays);
            var token = await _jwtHandler.ProtectAsync(_configuration.JwtTokenSigningKey, claims, new DateTimeOffset(DateTime.Now), new DateTimeOffset(expiryDate), "RenoWebsite");

            if (!string.IsNullOrEmpty(clientUserSM.ProfilePicturePath))
            {
                if (!_externalUserProcess.IsBase64String(clientUserSM.ProfilePicturePath)) // Check if it's already base64
                {
                    clientUserSM.ProfilePicturePath = await _externalUserProcess.ConvertToBase64(clientUserSM.ProfilePicturePath);
                }
            }
            else
            {
                clientUserSM.ProfilePicturePath = null;
            }

            var tokenResponse = new TokenResponseSM()
            {
                AccessToken = token,
                LoginUserDetails = clientUserSM,
                ExpiresUtc = expiryDate,
                ClientCompanyId = (int)clientUserSM.ClientCompanyDetailId,
            };
            if (isNewLogin == true)
            {
                tokenResponse.SuccessMessage = "Welcome aboard! Your account has been created successfully.";
            }
            else
            {
                tokenResponse.SuccessMessage = "Welcome back! You've logged in successfully.";
            }
            return Ok(ModelConverter.FormNewSuccessResponse(tokenResponse));
        }

        /* private async Task<ActionResult<ApiResponse<TokenResponseSM>>> CreateTokenForUser(ClientUserSM clientUserSM, string companyCode)
         {
             ICollection<Claim> claims = new List<Claim>()
             {
                 new Claim(ClaimTypes.Name,clientUserSM.LoginId),
                 new Claim(ClaimTypes.Role,clientUserSM.RoleType.ToString()),
                 new Claim(ClaimTypes.GivenName,$"{clientUserSM.FirstName} {clientUserSM.MiddleName} {clientUserSM.LastName}" ),
                 new Claim(ClaimTypes.Email,clientUserSM.EmailId),
                 new Claim(DomainConstants.ClaimsRoot.Claim_DbRecordId,clientUserSM.Id.ToString())
             };
             if (!string.IsNullOrWhiteSpace(companyCode))
             {
                 var compId = await _apiDbContext.ClientCompanyDetails.Where(x => x.CompanyCode == companyCode).Select(x => x.Id).FirstOrDefaultAsync();
                 if (compId != null || compId != 0)
                 {
                     clientUserSM.ClientCompanyDetailId = compId;
                 }
                 claims.Add(new Claim(DomainConstants.ClaimsRoot.Claim_ClientCode, companyCode));
                 claims.Add(new Claim(DomainConstants.ClaimsRoot.Claim_ClientId, clientUserSM.ClientCompanyDetailId.ToString()));
             }
             var expiryDate = DateTime.Now.AddDays(_configuration.DefaultTokenValidityDays);
             var token = await _jwtHandler.ProtectAsync(_configuration.JwtTokenSigningKey, claims, new DateTimeOffset(DateTime.Now), new DateTimeOffset(expiryDate), "CoreVision");
             var tokenResponse = new TokenResponseSM()
             {
                 AccessToken = token,
                 LoginUserDetails = clientUserSM,
                 ExpiresUtc = expiryDate,
                 ClientCompanyId = (int)clientUserSM.ClientCompanyDetailId,
             };
             return Ok(ModelConverter.FormNewSuccessResponse(tokenResponse));
         }*/
        #endregion
    }
}
