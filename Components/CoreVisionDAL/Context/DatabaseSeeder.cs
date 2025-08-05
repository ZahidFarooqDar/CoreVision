using CoreVisionDAL.Base;
using Microsoft.EntityFrameworkCore;
using CoreVisionDomainModels.Client;
using CoreVisionDomainModels.AppUser;
using System;
using CoreVisionDomainModels.Enums;
using CoreVisionDomainModels.v1.General.ScanCodes;
using CoreVisionDomainModels.v1.General.License;
using CoreVisionDomainModels.v1.Examination;

namespace CoreVisionDAL.Context
{
    public class DatabaseSeeder<T> where T : EfCoreContextRoot
    {
        #region Setup Database Seed Data
        public void SetupDatabaseWithSeedData(ModelBuilder modelBuilder)
        {
            var defaultCreatedBy = "SeedAdmin";
            SeedDummyCompanyData(modelBuilder, defaultCreatedBy);
        }

        #endregion Setup Database Seed Data

        #region Setup Database With Test Data
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
                SeedLicenseTypes(apiDb, defaultCreatedBy);
                SeedFeatures(apiDb, defaultCreatedBy);
                SeedTestLicenseTypes(apiDb, defaultCreatedBy);
                seedFeatureLicenseDetails(apiDb, defaultCreatedBy);
                SeedUserLicenseDetails(apiDb, defaultCreatedBy);
                SeedTestUserLicenseDetails(apiDb, defaultCreatedBy);
                SeedExamDetails(apiDb, defaultCreatedBy);
                SeedSubjectDetails(apiDb, defaultCreatedBy);
                SeedExamSubjectDetails(apiDb, defaultCreatedBy);
                SeedSubjectTopics(apiDb, defaultCreatedBy);
                SeedMCQs(apiDb, defaultCreatedBy);

                return true;
            }
            return false;
        }

        #endregion Setup Database With Test Data

        #region Data To Entities

        #region Companies
        private void SeedDummyCompanyData(ModelBuilder modelBuilder, string defaultCreatedBy)
        {
            var codeVisionCompany = new ClientCompanyDetailDM()
            {
                Id = 1,
                Name = "core-vision",
                CompanyCode = "123",
                Description = "Software Development Company",
                ContactEmail = "corevision@outlook.com",
                CompanyMobileNumber = "9876542341",
                CompanyWebsite = "www.corevision.com",
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
                EmailId = "saone@email.com",
                LastName = "One",
                LoginId = "super1",
                IsEmailConfirmed = true,
                LoginStatus = LoginStatusDM.Enabled,
                PhoneNumber = "1234567890",
                IsPhoneNumberConfirmed = true,
                PasswordHash = encryptorFunc("corevisionsuper1"),
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
                EmailId = "sysone@email.com",
                LastName = "One",
                LoginId = "system1",
                PhoneNumber = "1234567890",
                ProfilePicturePath = "wwwroot/content/loginusers/profile/profile.jpg",
                IsEmailConfirmed = true,
                LoginStatus = LoginStatusDM.Enabled,
                IsPhoneNumberConfirmed = true,
                PasswordHash = encryptorFunc("corevisionsystemadmin1"),
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
                FirstName = "Client",
                EmailId = "clientuser1@email.com",
                LastName = "One",
                LoginId = "clientuser1",
                IsEmailConfirmed = true,
                PhoneNumber = "1234567890",
                LoginStatus = LoginStatusDM.Enabled,
                IsPhoneNumberConfirmed = true,
                PasswordHash = encryptorFunc("pass123"),
                ProfilePicturePath = "wwwroot/content/loginusers/profile/profile.jpg",
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow
            };
            var cAdmin2 = new ClientUserDM()
            {
                ClientCompanyDetailId = 1,
                RoleType = RoleTypeDM.ClientAdmin,
                FirstName = "Client",
                EmailId = "clientuser2@email.com",
                LastName = "Two",
                LoginId = "clientuser2",
                IsEmailConfirmed = true,
                PhoneNumber = "1234567890",
                LoginStatus = LoginStatusDM.Enabled,
                IsPhoneNumberConfirmed = true,
                PasswordHash = encryptorFunc("pass123"),
                ProfilePicturePath = "wwwroot/content/loginusers/profile/profile.jpg",
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow
            };

            apiDb.ClientUsers.AddRange(cAdmin1,cAdmin2);
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

        #region Seed License Related Data

        #region Features
        private void SeedFeatures(ApiDbContext apiDb, string defaultCreatedBy)
        {

            var features = new List<FeatureDM>()
            {
                new FeatureDM()
                {
                    Title = "Text Summarization",
                    Description = "Generate concise summaries from text.",
                    FeatureCode = "CVSUM-2025",
                    UsageCount = 1,
                    isFeatureCountable = true,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(30),
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow,
                },
                new FeatureDM()
                {
                    Title = "Text Translation",
                    Description = "Convert text between languages",
                    FeatureCode = "CVTT-2025",
                    UsageCount = 0,
                    isFeatureCountable = false,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(30),
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow,
                },
                new FeatureDM()
                {
                    Title = "Image OCR Processing",
                    Description = "Extract text from images with high accuracy.",
                    FeatureCode = "CVTE-2025",
                    UsageCount = 1,
                    isFeatureCountable = true,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(30),
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow,
                },
                
                new FeatureDM()
                {
                    Title = "Audio Transcription",
                    Description = "Convert audio into text accurately",
                    FeatureCode = "CVAUD-2025",
                    UsageCount = 0,
                    isFeatureCountable = false,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(30),
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow,
                },
                new FeatureDM()
                {
                    Title = "Audio Summarization",
                    Description = "Generate concise summaries from audio",
                    FeatureCode = "CVAUDSUM-2025",
                    UsageCount = 0,
                    isFeatureCountable = false,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(30),
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow,
                },
                new FeatureDM()
                {
                    Title = "AI Image Generation",
                    Description = "Create images from text prompts with AI",
                    FeatureCode = "CVIMG-2025",
                    UsageCount = 0,
                    isFeatureCountable = false,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(30),
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow,
                },
                new FeatureDM()
                {
                    Title = "AI Story Generation",
                    Description = "Create Story from text prompts with AI",
                    FeatureCode = "CVSTORY-2025",
                    UsageCount = 0,
                    isFeatureCountable = false,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(30),
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow,
                },
                new FeatureDM()
                {
                    Title = "Barcode Generation",
                    Description = "Create Various Barcodes",
                    FeatureCode = "CVBARCODE-2025",
                    UsageCount = 0,
                    isFeatureCountable = false,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(30),
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow,
                },
                 new FeatureDM()
                {
                    Title = "AI Chat Assistant",
                    Description = "An intelligent AI-powered chat assistant that provides instant answers and engages in natural conversations",
                    FeatureCode = "CVCHAT-2025",
                    UsageCount = 0,
                    isFeatureCountable = false,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(30),
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow,
                }
            };

            apiDb.Features.AddRange(features);
            apiDb.SaveChanges();
        }

        #endregion Features

        #region License

        private void SeedLicenseTypes(ApiDbContext apiDb, string defaultCreatedBy)
        {
            var license1 = new LicenseTypeDM()
            {
                Title = "Trial",
                Description = "Core Vision Trial Plan",
                ValidityInDays = (1 * 15), 
                Amount = 0,
                LicenseTypeCode = "1TR2025CV0015",
                LicensePlan = LicensePlanDM.FifteenDays,
                ValidFor = RoleTypeDM.Unknown,
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow,
                StripePriceId = "0000",

            };
            var license2 = new LicenseTypeDM()
            {
                Title = "Basic",
                Description = "Core Vision Basic Plan",
                Amount = 199,
                LicenseTypeCode = "1BC2025CV0199",
                LicensePlan = LicensePlanDM.Monthly,
                ValidFor = RoleTypeDM.Unknown,
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow,
                StripePriceId = "price_1QvXBVIprUCdDPzTWZtXQTtT",

            };
            var license3 = new LicenseTypeDM()
            {
                Title = "Standard",
                Description = "Core Vision Standard Plan",
                ValidityInDays = (1 * 30),
                Amount = 299,
                LicenseTypeCode = "1SD2025CV0299",
                LicensePlan = LicensePlanDM.Monthly,
                ValidFor = RoleTypeDM.Unknown,
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow,
                StripePriceId = "price_1QvXEJIprUCdDPzTwiGqPvfv",

            };
            var license4 = new LicenseTypeDM()
            {
                Title = "Premium",
                Description = "Core Vision Premium Plan",
                ValidityInDays = (1 * 30),
                Amount = 399,
                LicenseTypeCode = "1PM2025CV0399",
                LicensePlan = LicensePlanDM.Monthly,
                ValidFor = RoleTypeDM.Unknown,
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow,
                StripePriceId = "price_1QvXFiIprUCdDPzTHefLTWUh",
            };
            
            apiDb.LicenseTypes.AddRange(license1, license2, license3, license4);

            apiDb.SaveChanges();
        }

        private void SeedTestLicenseTypes(ApiDbContext apiDb, string defaultCreatedBy)
        {
            var license1 = new TestLicenseTypeDM()
            {
                Title = "Trial",
                Description = "Trial version allowing users to attempt 2 tests — a great way to explore the platform.",
                TestCountValidity = 2,
                Amount = 0,
                LicenseTypeCode = "1TR2025TEST0002",
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow,
            };

            var license2 = new TestLicenseTypeDM()
            {
                Title = "Basic",
                Description = "Access 10 tests, including exam, subject, or topic-based — ideal for getting started.",
                TestCountValidity = 10,
                Amount = 49,
                LicenseTypeCode = "1TR2025TEST0010",
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow,
            };

            var license3 = new TestLicenseTypeDM()
            {
                Title = "Standard",
                Description = "Includes 30 test attempts across exams, subjects, and topics — suited for regular learners.",
                TestCountValidity = 30,
                Amount = 99,
                LicenseTypeCode = "1TR2025TEST0030",
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow,
            };

            var license4 = new TestLicenseTypeDM()
            {
                Title = "Premium",
                Description = "Unlock 50 full test sessions — covering exams, subjects, and topic-wise practice for thorough preparation.",
                TestCountValidity = 50,
                Amount = 149,
                LicenseTypeCode = "1TR2025TEST0050",
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow,
            };

            apiDb.TestLicenseTypes.AddRange(license1, license2, license3, license4);

            apiDb.SaveChanges();
        }

        #endregion License

        #region Seed Feature Details with License types

        private void seedFeatureLicenseDetails(ApiDbContext apiDb, string defaultCreatedBy)
        {
            var featureDetails = new List<FeatureDM_LicenseTypeDM>()
            {
                new () { LicenseTypeId = 1, FeatureId = 1},
                new () { LicenseTypeId = 1, FeatureId = 2},

                new () { LicenseTypeId = 2, FeatureId = 1},
                new () { LicenseTypeId = 2, FeatureId = 2},
                new () { LicenseTypeId = 2, FeatureId = 3},
                new () { LicenseTypeId = 2, FeatureId = 8},

                new () { LicenseTypeId = 3, FeatureId = 1},
                new () { LicenseTypeId = 3, FeatureId = 2},
                new () { LicenseTypeId = 3, FeatureId = 3},
                new () { LicenseTypeId = 3, FeatureId = 4},
                new () { LicenseTypeId = 3, FeatureId = 5},
                new () { LicenseTypeId = 3, FeatureId = 8},

                new () { LicenseTypeId = 4, FeatureId = 1},
                new () { LicenseTypeId = 4, FeatureId = 2},
                new () { LicenseTypeId = 4, FeatureId = 3},
                new () { LicenseTypeId = 4, FeatureId = 4},
                new () { LicenseTypeId = 4, FeatureId = 5},
                new () { LicenseTypeId = 4, FeatureId = 6},
                new () { LicenseTypeId = 4, FeatureId = 7},         
                new () { LicenseTypeId = 4, FeatureId = 8},       
                new () { LicenseTypeId = 4, FeatureId = 9},       

            };
            apiDb.LicenseFeatures.AddRange(featureDetails);
            apiDb.SaveChanges();
        }

        #endregion Seed Feature Details with License types

        #region UserLicenseDetail

        private async void SeedUserLicenseDetails(ApiDbContext apiDb, string defaultCreatedBy)
        {
            #region Client Admins Licenses


            var trialUserLicenseDetails1 = new UserLicenseDetailsDM()
            {
                SubscriptionPlanName = "Trial",
                LicenseTypeId = 1,
                ClientUserId = 1,
                DiscountInPercentage = 0,
                ActualPaidPrice = 0,
                ValidityInDays = 15,
                StartDateUTC = DateTime.UtcNow,
                ExpiryDateUTC = DateTime.UtcNow.AddDays(15),
                CancelledOn = DateTime.UtcNow.AddDays(15),
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow,
                ProductName = "Core Vision Trial Plan",
                Currency = "inr",
                StripePriceId = "0000",
                Status = "active",

            };            

            #endregion Client Admins Licenses         

            
            apiDb.UserLicenseDetails.Add(trialUserLicenseDetails1);
            apiDb.SaveChanges();
        }

        private async void SeedTestUserLicenseDetails(ApiDbContext apiDb, string defaultCreatedBy)
        {
            #region Client Admins Licenses


            var trialUserLicenseDetails1 = new UserTestLicenseDetailsDM()
            {
                SubscriptionPlanName = "Trial",
                TestLicenseTypeId = 1,
                ClientUserId = 1,
                DiscountInPercentage = 0,
                ActualPaidPrice = 0,
                TestCountValidity = 2,
                StartDateUTC = DateTime.UtcNow,
                CreatedBy = defaultCreatedBy,
                CreatedOnUTC = DateTime.UtcNow,
                PaymentMethod = PaymentMethodDM.Other,
                LicenseStatus = LicenseStatusDM.Active,

            };

            #endregion Client Admins Licenses         


            apiDb.UserTestLicenseDetails.Add(trialUserLicenseDetails1);
            apiDb.SaveChanges();
        }

        #endregion UserLicenseDetail

        #region Exams, Subjects, Topics

        #region Seed Exams Details
        private async void SeedExamDetails(ApiDbContext apiDb, string defaultCreatedBy)
        {
            var exams = new List<ExamDM>()
            {
                new ExamDM() { ExamName = "Naib Tehsildar",ExamDescription = "", ConductedBy = "JKSSB", CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow},
            };   
            apiDb.Exams.AddRange(exams);
            apiDb.SaveChanges();
        }

        #endregion Seed Exams Details

        #region Subjects
        private async void SeedSubjectDetails(ApiDbContext apiDb, string defaultCreatedBy)
        {
            var subjects = new List<SubjectDM>()
            {
                new()
                {
                    SubjectName = "General Knowledge & Current Affairs",
                    SubjectDescription = "Covers national, international and J&K-specific current events, history, polity, and general awareness.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    SubjectName = "Indian Polity",
                    SubjectDescription = "Fundamentals of Indian Constitution, governance, political system, and central/state structure.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    SubjectName = "Indian Economy",
                    SubjectDescription = "Basics of Indian economy, economic planning, budgeting, and economic issues in J&K.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    SubjectName = "Geography",
                    SubjectDescription = "Physical, social and economic geography of India and Jammu & Kashmir.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    SubjectName = "History & Culture",
                    SubjectDescription = "Major historical events of India and J&K, freedom struggle, art and culture.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    SubjectName = "Science and Technology",
                    SubjectDescription = "Everyday science, recent developments, and general scientific awareness.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    SubjectName = "Logical Reasoning & Analytical Ability",
                    SubjectDescription = "Covers puzzles, series, directions, coding-decoding, and logical reasoning skills.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    SubjectName = "Quantitative Aptitude",
                    SubjectDescription = "Basic arithmetic, algebra, geometry, and numerical problem-solving.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    SubjectName = "English Language",
                    SubjectDescription = "Grammar, comprehension, vocabulary, and writing ability.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    SubjectName = "Jammu & Kashmir Specific Knowledge",
                    SubjectDescription = "Culture, history, geography, economy and current affairs specific to J&K.",
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                }
            };

            apiDb.Subjects.AddRange(subjects);
            apiDb.SaveChanges();
        }

        #endregion Subjects

        #region Seed Exam Subject Details
        private async void SeedExamSubjectDetails(ApiDbContext apiDb, string defaultCreatedBy)
        {
            var examSubjects = new List<ExamSubjectsDM>()
            {
                new(){ ExamId = 1, SubjectId = 1 },
                new(){ ExamId = 1, SubjectId = 2 },
                new(){ ExamId = 1, SubjectId = 3 },
                new(){ ExamId = 1, SubjectId = 4 },
                new(){ ExamId = 1, SubjectId = 5 },
                new(){ ExamId = 1, SubjectId = 6 },
                new(){ ExamId = 1, SubjectId = 7 },
                new(){ ExamId = 1, SubjectId = 8 },
                new(){ ExamId = 1, SubjectId = 9 },
                new(){ ExamId = 1, SubjectId = 10 }
            };
            apiDb.ExamSubjects.AddRange(examSubjects);
            apiDb.SaveChanges();
        }

        #endregion Seed Exam Subject Details

        #region Seed Subject Topics
        private async void SeedSubjectTopics(ApiDbContext apiDb, string defaultCreatedBy)
        {
            var subjectTopics = new List<SubjectTopicDM>()
            {
                // SubjectId = 1 => General Knowledge & Current Affairs
                new() { TopicName = "National Current Affairs", TopicDescription = "Major events and developments across India.", SubjectId = 1, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "International Current Affairs", TopicDescription = "Important global news and issues.", SubjectId = 1, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "Jammu & Kashmir Current Affairs", TopicDescription = "Recent developments and events in J&K.", SubjectId = 1, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "General Awareness", TopicDescription = "Static general knowledge and awareness.", SubjectId = 1, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                
                // SubjectId = 2 => Indian Polity
                new() { TopicName = "Constitution of India", TopicDescription = "Features, amendments, and structure.", SubjectId = 2, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "Fundamental Rights and Duties", TopicDescription = "Citizen rights and constitutional obligations.", SubjectId = 2, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "Union Government", TopicDescription = "President, Parliament, Prime Minister and Council of Ministers.", SubjectId = 2, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "State Government", TopicDescription = "Governor, State Legislature, Chief Minister.", SubjectId = 2, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                
                // SubjectId = 3 => Indian Economy
                new() { TopicName = "Economic Planning in India", TopicDescription = "Five-year plans and NITI Aayog.", SubjectId = 3, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "Budget and Fiscal Policy", TopicDescription = "Union budget, taxation and expenditure policies.", SubjectId = 3, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "Inflation and Monetary Policy", TopicDescription = "RBI, repo rate, inflation control tools.", SubjectId = 3, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "Economy of Jammu & Kashmir", TopicDescription = "Sectors and growth of J&K's economy.", SubjectId = 3, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                
                // SubjectId = 4 => Geography
                new() { TopicName = "Physical Geography", TopicDescription = "Landforms, climate, and natural resources.", SubjectId = 4, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "Indian Geography", TopicDescription = "Rivers, mountains, states, and union territories.", SubjectId = 4, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "Jammu & Kashmir Geography", TopicDescription = "Valleys, rivers, and terrain of J&K.", SubjectId = 4, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                
                // SubjectId = 5 => History & Culture
                new() { TopicName = "Ancient Indian History", TopicDescription = "Indus Valley, Vedic Age and Mauryan Empire.", SubjectId = 5, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "Modern Indian History", TopicDescription = "British rule, freedom struggle and independence.", SubjectId = 5, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "Art and Culture", TopicDescription = "Indian classical dance, music, and architecture.", SubjectId = 5, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "J&K History and Culture", TopicDescription = "Dogra rule, folk art and religious sites.", SubjectId = 5, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                
                // SubjectId = 6 => Science and Technology
                new() { TopicName = "Everyday Science", TopicDescription = "Physics, chemistry and biology in daily life.", SubjectId = 6, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "Space and Defence Technology", TopicDescription = "ISRO, DRDO and space missions.", SubjectId = 6, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "IT and Communication", TopicDescription = "Internet, mobile communication and cyber security.", SubjectId = 6, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                
                // SubjectId = 7 => Logical Reasoning & Analytical Ability
                new() { TopicName = "Puzzles and Seating Arrangement", TopicDescription = "Logical puzzles and pattern recognition.", SubjectId = 7, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "Blood Relations and Coding-Decoding", TopicDescription = "Family-based problems and symbolic patterns.", SubjectId = 7, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "Direction Sense and Series", TopicDescription = "Movement-based and number/letter series.", SubjectId = 7, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                
                // SubjectId = 8 => Quantitative Aptitude
                new() { TopicName = "Arithmetic", TopicDescription = "Percentage, profit and loss, ratio and proportion.", SubjectId = 8, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "Algebra and Geometry", TopicDescription = "Equations, shapes, angles, and measurements.", SubjectId = 8, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "Data Interpretation", TopicDescription = "Bar graphs, pie charts, and tabular data.", SubjectId = 8, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                
                // SubjectId = 9 => English Language
                new() { TopicName = "Grammar and Sentence Correction", TopicDescription = "Tenses, prepositions, and sentence structure.", SubjectId = 9, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "Comprehension", TopicDescription = "Reading passages and answering questions.", SubjectId = 9, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "Vocabulary and Synonyms/Antonyms", TopicDescription = "Word meanings, synonyms and antonyms.", SubjectId = 9, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                
                // SubjectId = 10 => J&K Specific Knowledge
                new() { TopicName = "J&K Geography", TopicDescription = "Mountains, rivers, climate and districts.", SubjectId = 10, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "J&K Economy", TopicDescription = "Major industries, handicrafts and tourism.", SubjectId = 10, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow },
                new() { TopicName = "J&K Culture", TopicDescription = "Languages, festivals, music and traditions.", SubjectId = 10, CreatedBy = defaultCreatedBy, CreatedOnUTC = DateTime.UtcNow }
            };

            apiDb.SubjectTopics.AddRange(subjectTopics);
            apiDb.SaveChanges();
        }

        #endregion Seed Subject Topics

        #region Seed MCQs

        private async void SeedMCQs(ApiDbContext apiDb, string defaultCreatedBy)
        {
            var mcqs = new List<MCQDM>
            {
                #region Exams Mcq
                new()
                {
                    QuestionText = "Who conducts the Naib Tehsildar exam in Jammu and Kashmir?",
                    OptionA = "JKPSC", OptionB = "UPSC", OptionC = "JKSSB", OptionD = "SSC",
                    CorrectOption = "C",
                    Explanation = "JKSSB is responsible for conducting the Naib Tehsildar exam.",
                    ExamId = 1,
                    SubjectId = null,
                    SubjectTopicId = null,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    QuestionText = "Which article of the Indian Constitution deals with the Right to Equality?",
                    OptionA = "Article 14", OptionB = "Article 19", OptionC = "Article 21", OptionD = "Article 32",
                    CorrectOption = "A",
                    Explanation = "Article 14 provides for equality before law and equal protection of laws.",
                    SubjectId = null,
                    SubjectTopicId = null,
                    ExamId = 1,
                    CreatedOnUTC = DateTime.UtcNow,
                    CreatedBy = defaultCreatedBy,
                },
                new()
                {
                    QuestionText = "What is the capital of Jammu and Kashmir during summer?",
                    OptionA = "Jammu", OptionB = "Srinagar", OptionC = "Leh", OptionD = "Anantnag",
                    CorrectOption = "B",
                    Explanation = "Srinagar serves as the summer capital while Jammu is the winter capital.",
                    ExamId = 1,
                    SubjectId = null,
                    SubjectTopicId = null,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    QuestionText = "Which river is known as the lifeline of Jammu and Kashmir?",
                    OptionA = "Jhelum", OptionB = "Tawi", OptionC = "Chenab", OptionD = "Ravi",
                    CorrectOption = "A",
                    Explanation = "The Jhelum River plays a vital role in the economy and agriculture of the Kashmir Valley.",
                    ExamId = 1,
                    SubjectId = null,
                    SubjectTopicId = null,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    QuestionText = "Which Mughal emperor built the Shalimar Bagh in Srinagar?",
                    OptionA = "Akbar", OptionB = "Jahangir", OptionC = "Shah Jahan", OptionD = "Aurangzeb",
                    CorrectOption = "B",
                    Explanation = "Emperor Jahangir built the Shalimar Bagh for his wife Nur Jahan.",
                    ExamId = 1,
                    SubjectId = null,
                    SubjectTopicId = null,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    QuestionText = "Which of the following is NOT a Union Territory of India?",
                    OptionA = "Ladakh", OptionB = "Chandigarh", OptionC = "Sikkim", OptionD = "Puducherry",
                    CorrectOption = "C",
                    Explanation = "Sikkim is a full-fledged state, not a Union Territory.",
                    ExamId = 1,
                    SubjectId = null,
                    SubjectTopicId = null,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    QuestionText = "Who is the current Chief Election Commissioner of India? (as of 2025)",
                    OptionA = "Rajiv Kumar", OptionB = "Sushil Chandra", OptionC = "Sunil Arora", OptionD = "Ashok Lavasa",
                    CorrectOption = "A",
                    Explanation = "Rajiv Kumar is the current Chief Election Commissioner of India.",
                    ExamId = 1,
                    SubjectId = null,
                    SubjectTopicId = null,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    QuestionText = "Which gas is most abundant in the Earth’s atmosphere?",
                    OptionA = "Oxygen", OptionB = "Nitrogen", OptionC = "Carbon Dioxide", OptionD = "Hydrogen",
                    CorrectOption = "B",
                    Explanation = "Nitrogen constitutes about 78% of the Earth's atmosphere.",
                    ExamId = 1,
                    SubjectId = null,
                    SubjectTopicId = null,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    QuestionText = "Find the next number in the series: 2, 4, 8, 16, ?",
                    OptionA = "24", OptionB = "30", OptionC = "32", OptionD = "36",
                    CorrectOption = "C",
                    Explanation = "Each number is multiplied by 2 to get the next one: 2×2=4, 4×2=8, etc.",
                    ExamId = 1,
                    SubjectId = null,
                    SubjectTopicId = null,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    QuestionText = "Choose the correctly spelled word.",
                    OptionA = "Enviroment", OptionB = "Environment", OptionC = "Envirnment", OptionD = "Enviornment",
                    CorrectOption = "B",
                    Explanation = "The correct spelling is 'Environment'.",
                    ExamId = 1,
                    SubjectId = null,
                    SubjectTopicId = null,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                #endregion Exams Mcq

                #region Subject MCQs

                new()
                {
                    QuestionText = "Which Indian city was ranked highest in the Swachh Survekshan 2024?",
                    OptionA = "Indore", OptionB = "Surat", OptionC = "Bhopal", OptionD = "Ahmedabad",
                    CorrectOption = "A",
                    Explanation = "Indore secured the top rank in Swachh Survekshan 2024 for cleanliness.",
                    ExamId = null,
                    SubjectId = 1,
                    SubjectTopicId = null,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                 new()
                 {
                     QuestionText = "Who is the current President of India (as of 2025)?",
                     OptionA = "Ram Nath Kovind", OptionB = "Droupadi Murmu", OptionC = "Pranab Mukherjee", OptionD = "Narendra Modi",
                     CorrectOption = "B",
                     Explanation = "Droupadi Murmu became the 15th President of India in July 2022.",
                     ExamId = null,
                     SubjectId = 1,
                     SubjectTopicId = null,
                     CreatedBy = defaultCreatedBy,
                     CreatedOnUTC = DateTime.UtcNow
                 },
                 new()
                 {
                     QuestionText = "Which country recently became the 195th member of the United Nations?",
                     OptionA = "Kosovo", OptionB = "South Sudan", OptionC = "Palestine", OptionD = "None of the above",
                     CorrectOption = "D",
                     Explanation = "As of now, there are 193 member states in the UN; no new member was added recently.",
                     ExamId = null,
                     SubjectId = 1,
                     SubjectTopicId = null,
                     CreatedBy = defaultCreatedBy,
                     CreatedOnUTC = DateTime.UtcNow
                 },
                 new()
                 {
                     QuestionText = "Which state won the Best Performer award in Start-up India Ranking 2023?",
                     OptionA = "Gujarat", OptionB = "Karnataka", OptionC = "Maharashtra", OptionD = "Tamil Nadu",
                     CorrectOption = "A",
                     Explanation = "Gujarat was recognized as the best performer in Start-up India Ranking 2023.",
                     ExamId = null,
                     SubjectId = 1,
                     SubjectTopicId = null,
                     CreatedBy = defaultCreatedBy,
                     CreatedOnUTC = DateTime.UtcNow
                 },
                 new()
                 {
                     QuestionText = "What is the full form of G20?",
                     OptionA = "Group of Twenty", OptionB = "Global 20 Nations", OptionC = "Group of Top 20", OptionD = "None of these",
                     CorrectOption = "A",
                     Explanation = "G20 stands for Group of Twenty major economies.",
                     ExamId = null,
                     SubjectId = 1,
                     SubjectTopicId = null,
                     CreatedBy = defaultCreatedBy,
                     CreatedOnUTC = DateTime.UtcNow
                 },
                 new()
                 {
                     QuestionText = "Who is the current Chief Justice of India (as of July 2025)?",
                     OptionA = "Justice D. Y. Chandrachud", OptionB = "Justice N. V. Ramana", OptionC = "Justice U. U. Lalit", OptionD = "Justice Ranjan Gogoi",
                     CorrectOption = "A",
                     Explanation = "Justice D. Y. Chandrachud has been serving as the Chief Justice of India since November 2022.",
                     ExamId = null,
                     SubjectId = 1,
                     SubjectTopicId = null,
                     CreatedBy = defaultCreatedBy,
                     CreatedOnUTC = DateTime.UtcNow
                 },
                 new()
                 {
                     QuestionText = "Which country hosted the G20 Summit 2023?",
                     OptionA = "India", OptionB = "Indonesia", OptionC = "Brazil", OptionD = "Italy",
                     CorrectOption = "A",
                     Explanation = "India hosted the G20 Summit in 2023 under the theme 'One Earth, One Family, One Future'.",
                     ExamId = null,
                     SubjectId = 1,
                     SubjectTopicId = null,
                     CreatedBy = defaultCreatedBy,
                     CreatedOnUTC = DateTime.UtcNow
                 },
                 new()
                 {
                     QuestionText = "Which Indian state recently launched the 'Mukhyamantri Seekho-Kamao Yojana'?",
                     OptionA = "Uttar Pradesh", OptionB = "Madhya Pradesh", OptionC = "Bihar", OptionD = "Rajasthan",
                     CorrectOption = "B",
                     Explanation = "The scheme was launched by Madhya Pradesh to provide skill training and stipends.",
                     ExamId = null,
                     SubjectId = 1,
                     SubjectTopicId = null,
                     CreatedBy = defaultCreatedBy,
                     CreatedOnUTC = DateTime.UtcNow
                 },
                 new()
                 {
                     QuestionText = "What is the name of India’s lunar mission that successfully landed on the Moon in 2023?",
                     OptionA = "Chandrayaan-1", OptionB = "Chandrayaan-2", OptionC = "Chandrayaan-3", OptionD = "Vikram Lander",
                     CorrectOption = "C",
                     Explanation = "Chandrayaan-3 successfully landed on the Moon’s south pole in August 2023.",
                     ExamId = null,
                     SubjectId = 1,
                     SubjectTopicId = null,
                     CreatedBy = defaultCreatedBy,
                     CreatedOnUTC = DateTime.UtcNow
                 },
                 new()
                 {
                     QuestionText = "Which Indian airport was awarded the Best Airport in Asia-Pacific in 2023?",
                     OptionA = "Delhi Airport", OptionB = "Mumbai Airport", OptionC = "Bangalore Airport", OptionD = "Hyderabad Airport",
                     CorrectOption = "A",
                     Explanation = "Indira Gandhi International Airport, Delhi won the Best Airport in Asia-Pacific (40+ million passengers) award in 2023.",
                     ExamId = null,
                     SubjectId = 1,
                     SubjectTopicId = null,
                     CreatedBy = defaultCreatedBy,
                     CreatedOnUTC = DateTime.UtcNow
                 }, 
                 #endregion Subject MCQs

                #region Topic MCQs

                new()
                {
                    QuestionText = "Who is the current President of India as of 2025?",
                    OptionA = "Ram Nath Kovind", OptionB = "Pranab Mukherjee", OptionC = "Droupadi Murmu", OptionD = "Narendra Modi",
                    CorrectOption = "C",
                    Explanation = "Droupadi Murmu is the current President of India, having assumed office in July 2022.",
                    ExamId = null,
                    SubjectId = null,
                    SubjectTopicId = 1,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    QuestionText = "Which state recently launched the 'Mukhyamantri Seekho-Kamao Yojana' to promote youth employment?",
                    OptionA = "Rajasthan", OptionB = "Madhya Pradesh", OptionC = "Odisha", OptionD = "Bihar",
                    CorrectOption = "B",
                    Explanation = "Madhya Pradesh launched the 'Mukhyamantri Seekho-Kamao Yojana' in 2023 to enhance skill development.",
                    ExamId = null,
                    SubjectId = null,
                    SubjectTopicId = 1,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    QuestionText = "Which Indian city hosted the G20 Summit in 2023?",
                    OptionA = "New Delhi", OptionB = "Mumbai", OptionC = "Hyderabad", OptionD = "Kolkata",
                    CorrectOption = "A",
                    Explanation = "The G20 Summit 2023 was held in New Delhi, India.",
                    ExamId = null,
                    SubjectId = null,
                    SubjectTopicId = 1,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    QuestionText = "Who is the current Chief Justice of India as of 2025?",
                    OptionA = "N. V. Ramana", OptionB = "D. Y. Chandrachud", OptionC = "Ranjan Gogoi", OptionD = "U. U. Lalit",
                    CorrectOption = "B",
                    Explanation = "Justice D. Y. Chandrachud is the Chief Justice of India since 2022.",
                    ExamId = null,
                    SubjectId = null,
                    SubjectTopicId = 1,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    QuestionText = "Which Indian state became the first to implement a 'Green Hydrogen Policy'?",
                    OptionA = "Tamil Nadu", OptionB = "Kerala", OptionC = "Gujarat", OptionD = "Rajasthan",
                    CorrectOption = "D",
                    Explanation = "Rajasthan was the first Indian state to implement a Green Hydrogen Policy in 2023.",
                    ExamId = null,
                    SubjectId = null,
                    SubjectTopicId = 1,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    QuestionText = "Which Indian company launched the AI chatbot 'Jio GPT' in 2024?",
                    OptionA = "Tata", OptionB = "Infosys", OptionC = "Reliance Jio", OptionD = "HCL",
                    CorrectOption = "C",
                    Explanation = "Reliance Jio launched its AI chatbot 'Jio GPT' in early 2024.",
                    ExamId = null,
                    SubjectId = null,
                    SubjectTopicId = 1,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    QuestionText = "Which Indian has recently become the Chairperson of World Trade Organization (WTO) General Council?",
                    OptionA = "Anjali Sharma", OptionB = "Anwar Hussain Shaik", OptionC = "Rajeev Chandrasekhar", OptionD = "S. Jaishankar",
                    CorrectOption = "B",
                    Explanation = "Anwar Hussain Shaik was appointed as the Chairperson of the WTO General Council.",
                    ExamId = null,
                    SubjectId = null,
                    SubjectTopicId = 1,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    QuestionText = "Which Indian state launched 'Mission Shakti' for women's empowerment?",
                    OptionA = "Maharashtra", OptionB = "Uttar Pradesh", OptionC = "Odisha", OptionD = "Punjab",
                    CorrectOption = "C",
                    Explanation = "Odisha launched 'Mission Shakti' to empower women through SHGs and financial support.",
                    ExamId = null,
                    SubjectId = null,
                    SubjectTopicId = 1,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    QuestionText = "Who won the Padma Vibhushan in 2024 for public affairs?",
                    OptionA = "M. Venkaiah Naidu", OptionB = "Amitabh Bachchan", OptionC = "Narendra Modi", OptionD = "Ratan Tata",
                    CorrectOption = "A",
                    Explanation = "M. Venkaiah Naidu was awarded the Padma Vibhushan in 2024 for public affairs.",
                    ExamId = null,
                    SubjectId = null,
                    SubjectTopicId = 1,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                new()
                {
                    QuestionText = "Which Indian city was ranked highest in the Swachh Survekshan 2024?",
                    OptionA = "Indore", OptionB = "Surat", OptionC = "Bhopal", OptionD = "Ahmedabad",
                    CorrectOption = "A",
                    Explanation = "Indore secured the top rank in Swachh Survekshan 2024 for cleanliness.",
                    ExamId = null,
                    SubjectId = null,
                    SubjectTopicId = 1,
                    CreatedBy = defaultCreatedBy,
                    CreatedOnUTC = DateTime.UtcNow
                },
                #endregion Topic MCQs
            };

            apiDb.MCQs.AddRange(mcqs);
            apiDb.SaveChanges();
        }

        #endregion Seed MCQs

        #endregion Exams

        #endregion Seed License Related Data

        #endregion Data To Entities

    }
}
