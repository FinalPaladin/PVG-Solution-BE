using PVG.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVG.Domain.Constants
{
    public static class ConstRequestCustomer
    {
        public const string RC_FullName = "FullName";
        public const string RC_Phone = "Phone";
        public const string RC_BirthDay = "BirthDay";
        public const string RC_Age = "Age";
        public const string RC_Gender = "Gender";
        public const string RC_CCCD = "CCCD";
        public const string RC_CMND = "CMND";
        public const string RC_PlaceOfIssue = "PlaceOfIssue";
        public const string RC_DateOfIssue = "DateOfIssue";
        public const string RC_Nationality = "Nationality";
        public const string RC_Email = "Email";
        public const string RC_CurrentAdress = "Address";
        public const string RC_MaritalStatus = "MaritalStatus";
        public const string RC_CompanyName = "CompanyName";
        public const string RC_JobTitle = "JobTitle";
        public const string RC_Department = "Department";
        public const string RC_CompanyPhone = "CompanyPhone";
        public const string RC_SalaryIncome = "SalaryIncome";
        public const string RC_MonthIncome = "MonthIncome";
        public const string RC_LoanPurpose = "LoanPurpose";
        public const string RC_OutstandingLoansAtOtherBanks = "OutstandingLoansAtOtherBanks";
        public const string RC_LoanAmountRequested = "LoanAmountRequested";
        public const string RC_Collateral = "Collateral";
        public const string RC_PropertyAddress = "RedBookAddress";
        public const string RC_LoanProductType = "LoanProductType";
        public const string RC_OtherInfo = "OtherInfo";
        public const string RC_OtherIncome = "OrderIncome";

        public const string RC_PersonalInformation_Name = "Thông Tin Cá Nhân";
        public const string RC_ContactInformation_Name = "Thông Tin Liên Lạc";
        public const string RC_JobInformation_Name = "Thông Tin Việc Làm";
        public const string RC_CreditInformation_Name = "Thông Tin Tín Dụng";
        public const string RC_OtherInformation_Name = "Thông Tin Khác";
        public const string RC_FullName_Name = "Họ Và Tên";
        public const string RC_Phone_Name = "Số Điện Thoại";
        public const string RC_BirthDay_Name = "Ngày Sinh";
        public const string RC_Age_Name = "Tuổi";
        public const string RC_Gender_Name = "Giới Tính";
        public const string RC_CCCD_Name = "CCCD";
        public const string RC_CMND_Name = "CMND";
        public const string RC_PlaceOfIssue_Name = "Nơi Cấp";
        public const string RC_DateOfIssue_Name = "Ngày Cấp";
        public const string RC_Nationality_Name = "Quốc Tịch";
        public const string RC_Email_Name = "Email";
        public const string RC_CurrentAdress_Name = "Địa Chỉ Nơi Ở Hiện Tại";
        public const string RC_MaritalStatus_Name = "Tình Trạng Hôn Nhân";
        public const string RC_CompanyName_Name = "Tên Công Ty";
        public const string RC_JobTitle_Name = "Vị Trí";
        public const string RC_Department_Name = "Phòng Ban";
        public const string RC_CompanyPhone_Name = "Số Điện Thoại Công Ty";
        public const string RC_SalaryIncome_Name = "Thu Nhập Từ Lương";
        public const string RC_MonthIncome_Name = "Thu Nhập Hàng Tháng";
        public const string RC_LoanPurpose_Name = "Mục Đích Vay";
        public const string RC_OutstandingLoansAtOtherBanks_Name = "Dư Nợ Tại Các Ngân Hàng Khác";
        public const string RC_LoanAmountRequested_Name = "Số Tiền Muốn Vay";
        public const string RC_Collateral_Name = "Tài Sản Đảm Bảo";
        public const string RC_PropertyAddress_Name = "Địa chỉ Sổ Đỏ/Hồng";
        public const string RC_LoanProductType_Name = "Loại Sản Phẩm Vay";
        public const string RC_OtherInfo_Name = "Thông tin khác";
        public const string RC_OtherIncome_Name = "Thu nhập khác";

        public static List<RequestCustomerHeaderReport> ListRC = [
                        new RequestCustomerHeaderReport(){Name = RC_PersonalInformation_Name, Value = "title"},
                        new RequestCustomerHeaderReport(){Name = RC_FullName_Name, Value = "FullName"},
                        new RequestCustomerHeaderReport(){Name = RC_BirthDay_Name, Value = RC_BirthDay},
                        new RequestCustomerHeaderReport(){Name = RC_Age_Name, Value = RC_Age},
                        new RequestCustomerHeaderReport(){Name = RC_Gender_Name, Value = RC_Gender},
                        new RequestCustomerHeaderReport(){Name = RC_CCCD, Value = RC_CCCD},
                        new RequestCustomerHeaderReport(){Name = RC_CMND_Name, Value = RC_CMND},
                        new RequestCustomerHeaderReport(){Name = RC_PlaceOfIssue_Name, Value = RC_PlaceOfIssue},
                        new RequestCustomerHeaderReport(){Name = RC_DateOfIssue_Name, Value = RC_DateOfIssue},
                        new RequestCustomerHeaderReport(){Name = RC_Nationality_Name, Value = RC_Nationality},
                        new RequestCustomerHeaderReport(){Name = RC_MaritalStatus_Name, Value = RC_MaritalStatus},

                        new RequestCustomerHeaderReport(){Name = RC_ContactInformation_Name, Value = "title"},
                        new RequestCustomerHeaderReport(){Name = RC_Phone_Name, Value = "Phone"},
                        new RequestCustomerHeaderReport(){Name = RC_Email_Name, Value = RC_Email},
                        new RequestCustomerHeaderReport(){Name = RC_CurrentAdress_Name, Value = RC_CurrentAdress},

                        new RequestCustomerHeaderReport(){Name = RC_JobInformation_Name, Value = "title"},
                        new RequestCustomerHeaderReport(){Name = RC_CompanyName_Name, Value = RC_CompanyName},
                        new RequestCustomerHeaderReport(){Name = RC_JobTitle_Name, Value = RC_JobTitle},
                        new RequestCustomerHeaderReport(){Name = RC_Department_Name, Value = RC_Department},
                        new RequestCustomerHeaderReport(){Name = RC_CompanyPhone_Name, Value = RC_CompanyPhone},
                        new RequestCustomerHeaderReport(){Name = RC_SalaryIncome_Name, Value = RC_SalaryIncome},
                        new RequestCustomerHeaderReport(){Name = RC_MonthIncome_Name, Value = RC_MonthIncome},

                        new RequestCustomerHeaderReport(){Name = RC_CreditInformation_Name, Value = "title"},
                        new RequestCustomerHeaderReport(){Name = RC_LoanPurpose_Name, Value = RC_LoanPurpose},
                        new RequestCustomerHeaderReport(){Name = RC_OutstandingLoansAtOtherBanks_Name, Value = RC_OutstandingLoansAtOtherBanks},
                        new RequestCustomerHeaderReport(){Name = RC_LoanAmountRequested_Name, Value = RC_LoanAmountRequested},
                        new RequestCustomerHeaderReport(){Name = RC_Collateral_Name, Value = RC_Collateral},
                        new RequestCustomerHeaderReport(){Name = RC_PropertyAddress_Name, Value = RC_PropertyAddress},
                        new RequestCustomerHeaderReport(){Name = RC_OtherIncome_Name, Value = RC_OtherIncome},
                        new RequestCustomerHeaderReport(){Name = RC_OtherInfo_Name, Value = RC_OtherInfo},

                        new RequestCustomerHeaderReport(){Name = RC_OtherInformation_Name, Value = "title"},
                        new RequestCustomerHeaderReport(){Name = RC_LoanProductType_Name, Value = RC_LoanProductType},
                    ];
    }
}
