using System.ComponentModel.DataAnnotations;

namespace MetaFrm.Management.Razor.Models
{
    /// <summary>
    /// DictionaryModel
    /// </summary>
    public class DictionaryModel
    {
        /// <summary>
        /// DICTIONARY_ID
        /// </summary>
        public int? DICTIONARY_ID { get; set; }

        /// <summary>
        /// CODE
        /// </summary>
        [Required]
        [MinLength(3)]
        [Display(Name = "코드")]
        public string? CODE { get; set; }

        /// <summary>
        /// DESCRIPTION
        /// </summary>
        [Required]
        [MinLength(3)]
        [Display(Name = "설명")]
        public string? DESCRIPTION { get; set; }

        /// <summary>
        /// SQL
        /// </summary>
        [Required]
        [MinLength(3)]
        [Display(Name = "SQL")]
        public string? SQL { get; set; }

        /// <summary>
        /// WHERE_SQL
        /// </summary>
        [Display(Name = "Where SQL")]
        public string? WHERE_SQL { get; set; }

        /// <summary>
        /// ORDER_BY_SQL
        /// </summary>
        [Display(Name = "Order by SQL")]
        public string? ORDER_BY_SQL { get; set; }

        /// <summary>
        /// IS_CACHE
        /// </summary>
        [Display(Name = "캐시")]
        public string? IS_CACHE { get; set; }
    }
}