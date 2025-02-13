using CoreVisionDAL.Base;
using Microsoft.EntityFrameworkCore;
using CoreVisionDomainModels.Client;
using CoreVisionDomainModels.AppUser;
using System;
using CoreVisionDomainModels.Enums;
using CoreVisionDomainModels.v1.General.ScanCodes;

namespace CoreVisionDAL.Context
{
    public class DatabaseSeeder<T> where T : EfCoreContextRoot
    {
        public void SetupDatabaseWithSeedData(ModelBuilder modelBuilder)
        {
            var defaultCreatedBy = "SeedAdmin";
            SeedDummyCompanyData(modelBuilder, defaultCreatedBy);
        }
        //public bool SetupDatabaseWithTestData(T context, Func<string, string> encryptorFunc)
        public async Task<bool> SetupDatabaseWithTestData(T context, Func<string, string> encryptorFunc)
        {
            var defaultCreatedBy = "SeedAdmin";
            var defaultUpdatedBy = "UpdateAdmin";
            var apiDb = context as ApiDbContext;
            if (apiDb != null && apiDb.ApplicationUsers.Count() == 0)
            {
                SeedDummySuperAdminUsers(apiDb, defaultCreatedBy, defaultUpdatedBy, encryptorFunc);
                SeedDummySystemAdminUsers(apiDb, defaultCreatedBy, defaultUpdatedBy, encryptorFunc);
                SeedDummyClientAdminUsers(apiDb, defaultCreatedBy, defaultUpdatedBy, encryptorFunc);
                SeedScanCodesAdminUsers(apiDb, defaultCreatedBy);

                return true;
            }
            return false;
        }



        #region Data To Entities

        #region Companies
        private void SeedDummyCompanyData(ModelBuilder modelBuilder, string defaultCreatedBy)
        {
            var codeVisionCompany = new ClientCompanyDetailDM()
            {
                Id = 1,
                Name = "Code Vision",
                CompanyCode = "123",
                Description = "Software Development Company",
                ContactEmail = "codevision@outlook.com",
                CompanyMobileNumber = "9876542341",
                CompanyWebsite = "www.codevision.com",
                CompanyLogoPath = "wwwroot/content/companies/logos/company.jpg",
                CompanyDateOfEstablishment = new DateTime(1990, 1, 1),
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow
            };
            
            modelBuilder.Entity<ClientCompanyDetailDM>().HasData(codeVisionCompany);
        }

        #endregion Companies

        #region Users

        private void SeedDummySuperAdminUsers(ApiDbContext apiDb, string defaultCreatedBy, string defaultUpdatedBy, Func<string, string> encryptorFunc)
        {
            var superUser1 = new ApplicationUserDM()
            {
                RoleType = RoleTypeDM.SuperAdmin,
                FirstName = "Super",
                MiddleName = "Admin",
                EmailId = "saone@email.com",
                LastName = "One",
                LoginId = "super1",
                IsEmailConfirmed = true,
                LoginStatus = LoginStatusDM.Enabled,
                PhoneNumber = "1234567890",
                IsPhoneNumberConfirmed = true,
                PasswordHash = encryptorFunc("pass123"),
                ProfilePicturePath = "wwwroot/content/loginusers/profile/profile.jpg",
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow
            };
            
            apiDb.ApplicationUsers.Add(superUser1);
            apiDb.SaveChanges();
            
        }
        private void SeedDummySystemAdminUsers(ApiDbContext apiDb, string defaultCreatedBy, string defaultUpdatedBy, Func<string, string> encryptorFunc)
        {
            var sysUser1 = new ApplicationUserDM()
            {
                RoleType = RoleTypeDM.SystemAdmin,
                FirstName = "System",
                MiddleName = "Admin",
                EmailId = "sysone@email.com",
                LastName = "One",
                LoginId = "system1",
                PhoneNumber = "1234567890",
                ProfilePicturePath = "wwwroot/content/loginusers/profile/profile.jpg",
                IsEmailConfirmed = true,
                LoginStatus = LoginStatusDM.Enabled,
                IsPhoneNumberConfirmed = true,
                PasswordHash = encryptorFunc("pass123"),
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow
            };
            
            apiDb.ApplicationUsers.Add(sysUser1);
            apiDb.SaveChanges();
           
        }
        private void SeedDummyClientAdminUsers(ApiDbContext apiDb, string defaultCreatedBy, string defaultUpdatedBy, Func<string, string> encryptorFunc)
        {
            var cAdmin1 = new ClientUserDM()
            {
                ClientCompanyDetailId = 1,
                RoleType = RoleTypeDM.ClientAdmin,
                FirstName = "Company",
                MiddleName = "Admin",
                EmailId = "companyadmin1@email.com",
                LastName = "One",
                LoginId = "companyadmin1",
                IsEmailConfirmed = true,
                PhoneNumber = "1234567890",
                LoginStatus = LoginStatusDM.Enabled,
                IsPhoneNumberConfirmed = true,
                PasswordHash = encryptorFunc("pass123"),
                ProfilePicturePath = "wwwroot/content/loginusers/profile/profile.jpg",
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow
            };
            
            apiDb.ClientUsers.Add(cAdmin1);
            apiDb.SaveChanges();
            
        }


        #endregion Users

        #region Application Specific Tables

        #endregion Application Specific Tables

        #region Seed QRCode Data
        private void SeedScanCodesAdminUsers(ApiDbContext apiDb, string defaultCreatedBy)
        {
            var scanCodesData = new List<ScanCodesFormatDM>()
            {
                new ScanCodesFormatDM
                {
                    BarcodeFormat = "1",
                    BarcodeFormatName = "AZTEC",
                    Regex = @"^[A-Za-z0-9!@#$%^&*()_+=-]$",
                    ValidationMessage = "Enter letters (A-Z, a-z), numbers (0-9), and special characters.",
                    Description = "A flexible 2D barcode format that efficiently encodes large amounts of data, suitable for applications like mobile ticketing and boarding passes. It adapts well to varying data sizes.",
                    ErrorData = "The data provided may be too large to encode as an Aztec code. Please ensure your input consists of valid characters and try again.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new ScanCodesFormatDM
                {
                    BarcodeFormat = "2",
                    BarcodeFormatName = "CODABAR",
                    Regex = "^[0-9\\-\\$\\:/.+]+$",
                    ValidationMessage = "Enter numbers (0-9) and these special characters: - $ : / +.",
                    Description = "A 1D barcode typically used in libraries, blood banks, and photo labs. Accepts digits and symbols (- $ : / . +).",
                    ErrorData = "An error occurred while generating the barcode. Please ensure your input contains only numbers and the following symbols: - $ : / . +.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow

                },
                new ScanCodesFormatDM
                {
                    BarcodeFormat = "4",
                    BarcodeFormatName = "CODE_39",
                    Regex = @"^[A-Za-z0-9!@#$%^&*()_+=-]{1,42}$",
                    ValidationMessage = "Enter 1-42 characters: letters, numbers, and special characters.",
                    Description = "A widely used 1D barcode for inventory, used in automotive and defense. Accepts alphanumeric characters and symbols.",
                    ErrorData = "Input must be 42 characters or fewer and can include alphanumeric characters and special symbols.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow

                },
                new ScanCodesFormatDM
                {
                    BarcodeFormat = "8",
                    BarcodeFormatName = "CODE_93",
                    Regex = @"^[A-Za-z0-9!@#$%^&*()_+=-]{1,40}$",
                    ValidationMessage = "Enter 1-40 characters: letters, numbers, and special characters.",
                    Description = "A higher-density variant of Code 39, used in logistics and healthcare, supporting uppercase letters and symbols.",
                    ErrorData = "Input must be 40 characters or fewer and can include alphanumeric characters and special symbols.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow

                },
                new ScanCodesFormatDM
                {
                    BarcodeFormat = "16",
                    BarcodeFormatName = "CODE_128",
                    Regex = @"^[A-Za-z0-9!@#$%^&*()_+=-]$",
                    ValidationMessage = "Enter letters (A-Z, a-z), numbers (0-9), and special characters.",
                    Description = "A versatile 1D barcode for logistics and supply chain applications, supports all ASCII characters.",
                    ErrorData = "It seems the input data is too large to process. Please check that your entry is within acceptable limits and try again.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow

                },
                new ScanCodesFormatDM
                {
                    BarcodeFormat = "32",
                    BarcodeFormatName = "DATA_MATRIX",
                    Regex = @"^[A-Za-z0-9!@#$%^&*()_+=-]$",
                    ValidationMessage = "Enter letters (A-Z, a-z), numbers (0-9), and special characters.",
                    Description = "A versatile barcode for logistics and supply chain applications, supports all ASCII characters.",
                    ErrorData = "It seems the input data is too large to process. Please check that your entry is within acceptable limits and try again.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow

                },
                new ScanCodesFormatDM
                {
                    BarcodeFormat = "64",
                    BarcodeFormatName = "EAN_8",
                    Regex = "^\\d{7}$",
                    ValidationMessage = "Enter exactly 7 digits.",
                    Description = "A compact 1D barcode for small packages, commonly used in retail and product labeling. Requires 7 numeric digits.",
                    ErrorData = "Only 7 numeric digits are allowed, with the 8th digit being a checksum. The checksum is automatically calculated from the first 7 digits.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow

                },
                new ScanCodesFormatDM
                {
                    BarcodeFormat = "128",
                    BarcodeFormatName = "EAN_13",
                    Regex = "^\\d{12}$",
                    ValidationMessage = "Enter exactly 12 digits.",
                    Description = "The standard retail barcode worldwide, for product labeling. Requires 12 numeric digits.",
                    ErrorData = "Only 12 numeric digits are allowed, with the 13th digit being a checksum. The checksum is automatically calculated from the first 7 digits.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow

                },
                new ScanCodesFormatDM
                {
                    BarcodeFormat = "256",
                    BarcodeFormatName = "ITF",
                    Regex = @"^(?!.*\D)([0-9]{2}){1,20}$",
                    ValidationMessage = "Enter 2 to 40 digits, total digits must be even",
                    Description = "Interleaved Two of Five (ITF): A high-density 1D barcode commonly used in warehouses and on cartons, strictly allowing numeric digits in pairs, with a maximum length of 40 characters.",
                    ErrorData = "Input must be numeric digits only, with a maximum length of 40 characters and must consist of even digits.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow

                },
                new ScanCodesFormatDM
                {
                    BarcodeFormat = "1024",
                    BarcodeFormatName = "PDF_417",
                    Regex = "^^[A-Za-z0-9!@#$%^&*()_+=-]$",
                    ValidationMessage = "Enter letters (A-Z, a-z), numbers (0-9), and special characters.",
                    Description = "A versatile barcode for logistics and supply chain applications, supports all ASCII characters.",
                    ErrorData = "It seems the input data is too large to process. Please check that your entry is within acceptable limits and try again.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow

                },
                new ScanCodesFormatDM
                {
                    BarcodeFormat = "2048",
                    BarcodeFormatName = "QR_CODE",
                    Regex = "^^[A-Za-z0-9!@#$%^&*()_+=-]$",
                    ValidationMessage = "Enter letters (A-Z, a-z), numbers (0-9), and special characters.",
                    Description = "A versatile barcode for logistics and supply chain applications, supports all ASCII characters.",
                    ErrorData = "It seems the input data is too large to process. Please check that your entry is within acceptable limits and try again.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow

                },
                new ScanCodesFormatDM
                {
                    BarcodeFormat = "16384",
                    BarcodeFormatName = "UPC_A",
                    Regex = "^\\d{11}$",
                    ValidationMessage = "Enter exactly 11 digits.",
                    Description = "A 1D barcode standard for retail products in the USA, requiring 11 numeric digits.",
                    ErrorData = "Only 11 numeric digits are allowed, with the 12th digit being a checksum. The checksum is automatically calculated from the first 11 digits.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow

                },
                new ScanCodesFormatDM
                {
                    BarcodeFormat = "32768",
                    BarcodeFormatName = "UPC_E",
                    Regex = "^\\d{7}$",
                    ValidationMessage = "Enter exactly 7 digits.",
                    Description = "A compact version of UPC for smaller items in retail, requiring 7 numeric digits.",
                    ErrorData = "Only 7 numeric digits are allowed, with the 8th digit being a checksum. The checksum is automatically calculated from the first 7 digits.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow

                },
                new ScanCodesFormatDM
                {
                    BarcodeFormat = "131072",
                    BarcodeFormatName = "MSI",
                    Regex = "^\\d+$",
                    ValidationMessage = "Enter Numbers (0-9)",
                    Description = "A numeric-only barcode often used in inventory control and storage applications.",
                    ErrorData = "Input must be numeric and within the acceptable size limit",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow

                },
                new ScanCodesFormatDM
                {
                    BarcodeFormat = "262144",
                    BarcodeFormatName = "PLESSEY",
                    Regex = "^\\d+$",
                    ValidationMessage = "Enter Numbers (0-9)",
                    Description = "A barcode primarily used in libraries and warehouses for numeric data storage and retrieval.",
                    ErrorData = "Input must be numeric and within the acceptable size limit",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow

                },
            };

            apiDb.ScanCodes.AddRange(scanCodesData);
            apiDb.SaveChanges();
        }

        #endregion Seed QRCode Data

        #endregion Data To Entities

    }
}
