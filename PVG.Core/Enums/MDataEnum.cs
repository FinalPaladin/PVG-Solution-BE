using System.ComponentModel.DataAnnotations;

namespace PVG.Domain.Enums
{
    public enum MDataEnum_Group
    {
        [Display(Name = "Danh mục sản phẩm")]
        PRODUCT_CATEGORY = 1,
        [Display(Name = "Mức vay")]
        PRODUCT_AMOUNT = 2,
        [Display(Name = "Thời hạn vay")]
        PRODUCT_TIME = 3,
        [Display(Name = "Chi tiết danh mục sản phẩm")]
        PRODUCT_CATEGORY_DETAIL = 4
    }

    public enum NewsTypeEnum
    {
        News = 1,
        FAQs = 2,
        Slider = 3
    }
}