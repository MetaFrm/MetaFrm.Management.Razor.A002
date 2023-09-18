using MetaFrm.Database;
using MetaFrm.Extensions;
using MetaFrm.Management.Razor.Models;
using MetaFrm.Management.Razor.ViewModels;
using MetaFrm.Razor.Essentials;
using MetaFrm.Service;
using MetaFrm.Web.Bootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MetaFrm.Management.Razor
{
    /// <summary>
    /// A002
    /// </summary>
    public partial class A002
    {
        #region Variable
        internal A002ViewModel A002ViewModel { get; set; } = Factory.CreateViewModel<A002ViewModel>();

        internal DataGridControl<DictionaryModel>? DataGridControl;

        internal DictionaryModel SelectItem = new();

        internal int? PagingSize = null;
        #endregion


        #region Init
        /// <summary>
        /// OnInitializedAsync
        /// </summary>
        /// <returns></returns>
        protected override async Task<Task> OnInitializedAsync()
        {
            try
            {
                if (this.JSRuntime != null)
                {
                    System.Drawing.Size browserDimension = await this.JSRuntime.InvokeAsync<System.Drawing.Size>("getDimensions", null);
                    int? tmp = (browserDimension.Height - 210) / this.DataGridControl.HeaderHeight;
                    this.PagingSize = tmp < 5 ? 5 : tmp;
                }
            }
            catch (Exception) { }

            return base.OnInitializedAsync();
        }

        /// <summary>
        /// OnAfterRenderAsync
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (!this.AuthState.IsLogin())
                    this.Navigation?.NavigateTo("/", true);

                this.A002ViewModel = await this.GetSession<A002ViewModel>(nameof(this.A002ViewModel));

                this.Search();

                this.StateHasChanged();
            }
        }
        #endregion


        #region IO
        private void New()
        {
            this.SelectItem = new();
        }

        private void OnSearch()
        {
            if (this.DataGridControl != null)
                this.DataGridControl.CurrentPageNumber = 1;

            this.Search();
        }
        private void Search()
        {
            Response response;

            try
            {
                if (this.A002ViewModel.IsBusy) return;

                this.A002ViewModel.IsBusy = true;

                ServiceData serviceData = new()
                {
                    Token = this.AuthState.Token()
                };
                serviceData["1"].CommandText = this.GetAttribute("Search");
                serviceData["1"].AddParameter(nameof(this.A002ViewModel.SearchModel.SEARCH_TEXT), DbType.NVarChar, 4000, this.A002ViewModel.SearchModel.SEARCH_TEXT);
                serviceData["1"].AddParameter("USER_ID", DbType.Int, 3, this.AuthState.UserID());
                serviceData["1"].AddParameter("PAGE_NO", DbType.Int, 3, this.DataGridControl != null ? this.DataGridControl.CurrentPageNumber : 1);
                serviceData["1"].AddParameter("PAGE_SIZE", DbType.Int, 3, this.DataGridControl != null && this.DataGridControl.PagingEnabled ? this.DataGridControl.PagingSize : int.MaxValue);

                response = serviceData.ServiceRequest(serviceData);

                if (response.Status == Status.OK)
                {
                    this.A002ViewModel.SelectResultModel.Clear();

                    if (response.DataSet != null && response.DataSet.DataTables.Count > 0)
                    {
                        var orderResult = response.DataSet.DataTables[0].DataRows.OrderBy(x => x.String(nameof(DictionaryModel.CODE)));

                        foreach (var datarow in orderResult)
                        {
                            this.A002ViewModel.SelectResultModel.Add(new DictionaryModel
                            {
                                DICTIONARY_ID = datarow.Int(nameof(DictionaryModel.DICTIONARY_ID)),
                                CODE = datarow.String(nameof(DictionaryModel.CODE)),
                                DESCRIPTION = datarow.String(nameof(DictionaryModel.DESCRIPTION)),
                                SQL = datarow.String(nameof(DictionaryModel.SQL)),
                                WHERE_SQL = datarow.String(nameof(DictionaryModel.WHERE_SQL)),
                                ORDER_BY_SQL = datarow.String(nameof(DictionaryModel.ORDER_BY_SQL)),
                            });
                        }

                        //this.DataGridControl?.SortInit(this.ColumnDefinitions, nameof(SelectResultModel.NAMESPACE), SortDirection.Ascending);
                        this.DataGridControl?.SortData();
                        //this.DataGridControl.Pages = new int[] { 1, 2, 3, 4 };
                    }
                }
                else
                {
                    if (response.Message != null)
                    {
                        this.ModalShow("Warning", response.Message, new() { { "Ok", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("An Exception has occurred.", e.Message, new() { { "Ok", Btn.Danger } }, null);
            }
            finally
            {
                this.A002ViewModel.IsBusy = false;
                this.SetSession(nameof(A002ViewModel), this.A002ViewModel);
            }
        }

        private void Save()
        {
            Response? response;
            string? value;

            response = null;

            try
            {
                if (this.A002ViewModel.IsBusy)
                    return;

                if (this.SelectItem == null)
                    return;

                this.A002ViewModel.IsBusy = true;

                ServiceData serviceData = new()
                {
                    TransactionScope = true,
                    Token = this.AuthState.Token()
                };
                serviceData["1"].CommandText = this.GetAttribute("Save");
                serviceData["1"].AddParameter(nameof(this.SelectItem.DICTIONARY_ID), DbType.Int, 3, "2", nameof(this.SelectItem.DICTIONARY_ID), this.SelectItem.DICTIONARY_ID);
                serviceData["1"].AddParameter(nameof(this.SelectItem.CODE), DbType.NVarChar, 50, this.SelectItem.CODE);
                serviceData["1"].AddParameter(nameof(this.SelectItem.DESCRIPTION), DbType.NVarChar, 100, this.SelectItem.DESCRIPTION);
                serviceData["1"].AddParameter(nameof(this.SelectItem.SQL), DbType.VarChar, 8000, this.SelectItem.SQL);
                serviceData["1"].AddParameter(nameof(this.SelectItem.WHERE_SQL), DbType.NVarChar, 1000, this.SelectItem.WHERE_SQL);
                serviceData["1"].AddParameter(nameof(this.SelectItem.ORDER_BY_SQL), DbType.NVarChar, 1000, this.SelectItem.ORDER_BY_SQL);
                serviceData["1"].AddParameter("USER_ID", DbType.Int, 3, this.AuthState.UserID());

                response = serviceData.ServiceRequest(serviceData);

                if (response.Status == Status.OK)
                {
                    if (response.DataSet != null && response.DataSet.DataTables.Count > 2 && response.DataSet.DataTables[2].DataRows.Count > 0 && this.SelectItem != null && this.SelectItem.DICTIONARY_ID == null)
                    {
                        value = response.DataSet.DataTables[2].DataRows[0].String("Value");

                        if (value != null)
                            this.SelectItem.DICTIONARY_ID = value.ToInt();
                    }

                    this.ToastShow("Completed", $"{this.GetAttribute("Title")} registered successfully.", Alert.ToastDuration.Long);
                }
                else
                {
                    if (response.Message != null)
                    {
                        this.ModalShow("Warning", response.Message, new() { { "Ok", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("An Exception has occurred.", e.Message, new() { { "Ok", Btn.Danger } }, null);
            }
            finally
            {
                this.A002ViewModel.IsBusy = false;

                if (response != null && response.Status == Status.OK)
                {
                    this.Search();
                    this.StateHasChanged();
                }
            }
        }

        private void Delete()
        {
            this.ModalShow($"Question", "Do you want to delete?", new() { { "Delete", Btn.Danger }, { "Cancel", Btn.Primary } }, EventCallback.Factory.Create<string>(this, DeleteAction));
        }
        private void DeleteAction(string action)
        {
            Response? response;

            response = null;

            try
            {
                if (action != "Delete") return;

                if (this.A002ViewModel.IsBusy) return;

                if (this.SelectItem == null) return;

                this.A002ViewModel.IsBusy = true;

                if (this.SelectItem.DICTIONARY_ID == null || this.SelectItem.DICTIONARY_ID <= 0)
                {
                    this.ToastShow("Warning", $"Please select a {this.GetAttribute("Title")} to delete.", Alert.ToastDuration.Long);
                    return;
                }

                ServiceData serviceData = new()
                {
                    TransactionScope = true,
                    Token = this.AuthState.Token()
                };
                serviceData["1"].CommandText = this.GetAttribute("Delete");
                serviceData["1"].AddParameter(nameof(this.SelectItem.DICTIONARY_ID), DbType.Int, 3, this.SelectItem.DICTIONARY_ID);
                serviceData["1"].AddParameter("USER_ID", DbType.Int, 3, this.AuthState.UserID());

                response = serviceData.ServiceRequest(serviceData);

                if (response.Status == Status.OK)
                {
                    this.New();
                    this.ToastShow("Completed", $"{this.GetAttribute("Title")} deleted successfully.", Alert.ToastDuration.Long);
                }
                else
                {
                    if (response.Message != null)
                    {
                        this.ModalShow("Warning", response.Message, new() { { "Ok", Btn.Warning } }, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.ModalShow("An Exception has occurred.", e.Message, new() { { "Ok", Btn.Danger } }, null);
            }
            finally
            {
                this.A002ViewModel.IsBusy = false;

                if (response != null && response.Status == Status.OK)
                {
                    this.Search();
                    this.StateHasChanged();
                }
            }
        }
        #endregion


        #region Event
        private void SearchKeydown(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                this.OnSearch();
            }
        }

        private void RowModify(DictionaryModel item)
        {
            this.SelectItem = new()
            {
                DICTIONARY_ID = item.DICTIONARY_ID,
                CODE = item.CODE,
                DESCRIPTION = item.DESCRIPTION,
                SQL = item.SQL,
                WHERE_SQL = item.WHERE_SQL,
                ORDER_BY_SQL = item.ORDER_BY_SQL,
            };
        }

        private void Copy()
        {
            if (this.SelectItem != null)
            {
                this.SelectItem.DICTIONARY_ID = null;
            }
        }
        #endregion
    }
}