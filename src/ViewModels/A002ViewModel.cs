using MetaFrm.Management.Razor.Models;
using MetaFrm.MVVM;

namespace MetaFrm.Management.Razor.ViewModels
{
    /// <summary>
    /// A001ViewModel
    /// </summary>
    public partial class A002ViewModel : BaseViewModel
    {
        /// <summary>
        /// SearchModel
        /// </summary>
        public SearchModel SearchModel { get; set; } = new();

        /// <summary>
        /// SelectResultModel
        /// </summary>
        public List<DictionaryModel> SelectResultModel { get; set; } = new List<DictionaryModel>();

        /// <summary>
        /// C001ViewModel
        /// </summary>
        public A002ViewModel()
        {

        }
    }
}