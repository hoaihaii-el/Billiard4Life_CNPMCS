using Billiard4Life.DataProvider;
using Billiard4Life.View;
using System.Windows.Controls;
using System.Windows.Data;
using System;

namespace Billiard4Life.ViewModel
{
    public class KhuyenMaiViewModel : BaseViewModel
    {
        public KhuyenMaiViewModel()
        {
            KhuyenMais = KhuyenMaiDP.Flag.GetKhuyenMais();
            KhuyenMaiDP.Flag.AutoUpdateStatusKhuyenMai(KhuyenMais);
            AddKhuyenMaiItem = new KhuyenMai();
            SelectedItem = new KhuyenMai();
            LoadTrangThai();
            khuyenmai_view = new CollectionViewSource();
            khuyenmai_view.Source = KhuyenMais;
            khuyenmai_view.Filter += KhuyenMais_Filter;
            AddKhuyenMai_Command = new RelayCommand<object>((p) => true, (p) =>
            {
                KhuyenMai_Them view = new KhuyenMai_Them();
                view.DataContext = this;
                AddKhuyenMaiItem.MAKM = KhuyenMaiDP.Flag.AutoIDKhuyenMai();
                AddKhuyenMaiItem.TrangThai = "Đang diễn ra";
                view.ShowDialog();
            });

            EditKhuyenMai_Command = new RelayCommand<object>((p) => true, (p) =>
            {
                KhuyenMai_Sua view = new KhuyenMai_Sua();
                view.DataContext = this;
                view.ShowDialog();
            });

            SelectAllKhuyenMai_Command = new RelayCommand<CheckBox>((p) => true, (p) =>
            {
                if (p.IsChecked == true)
                {

                }
            });

            DeleteKhuyenMai_Command = new RelayCommand<object>((p) => true, (p) =>
            {
                if (IsAnyKhuyenMaiSelected() == false)
                {
                    MyMessageBox msb = new MyMessageBox("Hãy tích vào khuyến mãi mà bạn muốn xóa!");
                    msb.ShowDialog();
                }
                else
                {
                    string mess = String.Empty;
                    try
                    {
                        foreach (KhuyenMai item in KhuyenMais.ToList())
                        {
                            if (item.IsSelected == true)
                            {
                                KhuyenMais.Remove(item);
                                KhuyenMaiDP.Flag.DeleteKhuyenmai(item.MAKM);
                            }
                        }
                        mess = "Xoá thành công";
                    }
                    catch (Exception ex)
                    {
                        mess = ex.Message;
                    }
                    finally
                    {
                        MyMessageBox msb = new MyMessageBox(mess);
                        msb.Show();
                    }
                }
            });

            AddKhuyenMaiItem_Command = new RelayCommand<Grid>((p) =>
            {
                if (AddKhuyenMaiItem.isNullOrEmpty())
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                string mess = "";
                try
                {
                    if (KhuyenMaiIsListed())
                    {
                        mess = "Thông tin khuyến mãi này đã bị trùng (Mã khuyến mãi)";
                        return;
                    }
                    if (!NgayBDBeHonNgayKT(AddKhuyenMaiItem.NgayBatDau, AddKhuyenMaiItem.NGayKetThuc))
                    {
                        mess = "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu";
                        return;
                    }
                    if (!percentageValidation(AddKhuyenMaiItem.GiamGia))
                    {
                        mess = "Giá trị giảm phải lớn hơn 0 và bé hơn hoặc bằng 100";
                        return;
                    }
                    KhuyenMaiDP.Flag.AddKhuyenMai(AddKhuyenMaiItem);
                    KhuyenMais.Add(new KhuyenMai(AddKhuyenMaiItem.MAKM, AddKhuyenMaiItem.TenKM, AddKhuyenMaiItem.MucApDung, AddKhuyenMaiItem.TrangThai, AddKhuyenMaiItem.NgayBatDau, AddKhuyenMaiItem.NGayKetThuc, AddKhuyenMaiItem.MoTa, AddKhuyenMaiItem.GiamGia));
                    AddKhuyenMaiItem.clear();
                    mess = "Thêm thành công";
                }
                catch (Exception ex)
                {
                    mess = ex.Message;
                }
                finally
                {
                    MyMessageBox msb = new MyMessageBox(mess);
                    msb.Show();
                }
            });

            EditKhuyenMaiItem_Command = new RelayCommand<object>((p) =>
            {
                if (SelectedItem.isNullOrEmpty())
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                string mess = String.Empty;
                try
                {
                    KhuyenMaiDP.Flag.UpdateKhuyenMai(SelectedItem);
                    if (!NgayBDBeHonNgayKT(SelectedItem.NgayBatDau, SelectedItem.NGayKetThuc))
                    {
                        mess = "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu";
                        return;
                    }
                    if (!percentageValidation(SelectedItem.GiamGia))
                    {
                        mess = "Giá trị giảm phải lớn hơn 0 và bé hơn hoặc bằng 100";
                        return;
                    }
                    mess = "Sửa thành công";
                    OnPropertyChanged();
                }
                catch (Exception ex)
                {
                    mess = ex.Message;
                }
                finally
                {
                    MyMessageBox msb = new MyMessageBox(mess);
                    msb.Show();
                }
            });
        }
    }
}
