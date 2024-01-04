﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Billiard4Life.DataProvider;
using Billiard4Life.Models;
using Billiard4Life.View;
using System.Windows.Data;
using System.ComponentModel;
using Diacritics.Extensions;
using System.Data.SqlClient;
using System.Diagnostics;
using Button = System.Windows.Controls.Button;
using Kho = Billiard4Life.Models.Kho;

namespace Billiard4Life.ViewModel
{
    public class MenuAdminViewModel : BaseViewModel
    {
        public MenuAdminViewModel()
        {
            LoadMenu();
            Ingredients = MenuDP.Flag.GetIngredients();
            Ingredients_ForDishes = new ObservableCollection<ChiTietMon>();
            Deleted_Ingredients = new ObservableCollection<ChiTietMon>();
            _menuItemsView = new CollectionViewSource();
            _menuItemsView.Source = MenuItems;
            _menuItemsView.Filter += MenuItems_Filter;
            _ingredientsView = new CollectionViewSource();
            _ingredientsView.Source = Ingredients;
            _ingredientsView.Filter += Ingredients_Filter;
            addItem = new Models.MenuItem();
            MenuItem = new Models.MenuItem();
            AddItem.FoodImage = converting("pack://application:,,,/images/menu_default_image.jpg");
            

        }

        #region attributes
        private ObservableCollection<Models.MenuItem> _menuitems;
        private ObservableCollection<Kho> _ingredients;
        private ObservableCollection<ChiTietMon> _ingredients_ForDishes;
        private ObservableCollection<ChiTietMon> _deletedIngredients;
        private string _filterText;
        private string _ingreFilterText;
        private CollectionViewSource _menuItemsView;
        private CollectionViewSource _ingredientsView;
        private Models.MenuItem _menuitem;
        private Models.Kho _selected_Ingredient;
        private Models.MenuItem addItem;
        private bool IsAdding;
        private Visibility tabDish, tabIngredient;
        private bool _dishHasBeenAdded;
        private TabItem _selectedTab;
        private bool _isFirstTabVisible = true;
        public bool IsFirstTabVisible
        {
            get { return _isFirstTabVisible; }
            set
            {
                _isFirstTabVisible = value;
                OnPropertyChanged(nameof(IsFirstTabVisible));
            }
        }
        #endregion

        #region properties
        public ObservableCollection<Models.MenuItem> MenuItems { get { return _menuitems; } set { _menuitems = value; OnPropertyChanged(); } }
        public ObservableCollection<Kho> Ingredients { get { return _ingredients; } set { _ingredients = value; OnPropertyChanged(); } }
        public ObservableCollection<ChiTietMon> Ingredients_ForDishes { get { return _ingredients_ForDishes; } set { _ingredients_ForDishes = value; OnPropertyChanged(); } }
        public ObservableCollection<ChiTietMon> Deleted_Ingredients { get { return _deletedIngredients; } set { _deletedIngredients = value; OnPropertyChanged(); } }
        public Models.MenuItem MenuItem { get { return _menuitem; } set { _menuitem = value; OnPropertyChanged(); } }
        public Models.MenuItem AddItem { get { return addItem; } set { addItem = value; OnPropertyChanged(); } }
        public Kho Selected_Ingredient { get { return _selected_Ingredient; } set { _selected_Ingredient = value; OnPropertyChanged(); } }
        public bool DishHasBeenAdded { get { return _dishHasBeenAdded; } set { _dishHasBeenAdded = value; OnPropertyChanged(); } }
        public string FilterText { get { return _filterText; } set { _filterText = value; this._menuItemsView.View.Refresh(); OnPropertyChanged(); } }
        public string IngreFilterText { get { return _ingreFilterText; } set { _ingreFilterText = value; this._ingredientsView.View.Refresh(); OnPropertyChanged(); } }
        public Visibility TabDish { get { return tabDish; } set { tabDish = value; OnPropertyChanged(); } }
        public Visibility TabIngredient { get { return tabIngredient; } set { tabIngredient = value; OnPropertyChanged(); } }
        public TabItem SelectedTab { get { return _selectedTab; } set { _selectedTab = value; OnPropertyChanged(); } }
        public ICollectionView MenuItemCollection
        {
            get
            {
                return this._menuItemsView.View;
            }
        }
        public ICollectionView IngredientCollection
        {
            get
            {
                return this._ingredientsView.View;
            }
        }
        #endregion

        #region commands
        public ICommand AddDish_Command { get; set; }
        public ICommand RemoveDish_Command { get; set; }
        public ICommand AddImage_Command { get; set; }
        public ICommand EditIngredient_Command { get; set; }
        public ICommand AddOneMenuDish { get; set; }
        #endregion

        #region complementary functions
        public BitmapImage converting(string ur)
        {
            BitmapImage bmi = new BitmapImage();
            bmi.BeginInit();
            bmi.CacheOption = BitmapCacheOption.OnLoad;
            bmi.UriSource = new Uri(ur);
            bmi.EndInit();

            return bmi;
        }
        public bool IsListedInIngredientList(string TenNL)
        {
            if (Ingredients_ForDishes.Count == 0) return false;
            foreach (ChiTietMon ctm in Ingredients_ForDishes)
            {
                if (ctm.TenNL.CompareTo(TenNL) == 0)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsListedInMenuList(string MaMon)
        {
            if (MenuItems.Count == 0) return false;
            foreach (Models.MenuItem mi in MenuItems)
            {
                if (string.Compare(mi.ID, MaMon) == 0)
                    return true;
            }
            return false;
        }
        public bool CheckIfIngredientListInclude0InQuantity()
        {
            foreach (ChiTietMon ctm in Ingredients_ForDishes)
            {
                if (ctm.SoLuong <= 0)
                {
                    return true;
                }
            }
            return false;
        }
        public void MenuItems_Filter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(FilterText))
            {
                e.Accepted = true;
                return;
            }

            Models.MenuItem item = e.Item as Models.MenuItem;
            if (item.FoodName.RemoveDiacritics().ToLower().Contains(FilterText.RemoveDiacritics().ToLower()))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }
        private void Ingredients_Filter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(IngreFilterText))
            {
                e.Accepted = true;
                return;
            }

            Models.Kho item = e.Item as Models.Kho;
            if (item.TenSanPham.RemoveDiacritics().ToLower().Contains(IngreFilterText.RemoveDiacritics().ToLower()))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }
        private async Task LoadMenu()
        {
            _menuitems = await MenuDP.Flag.ConvertToCollection();
        }
        private bool IsAnyIngredientSelected()
        {
            foreach (Kho ingre in IngredientCollection)
            {
                if (ingre.DuocChon == true) return true;
            }
            return false;
        }
        private bool IsIngredientsValid()
        {
            foreach (Kho ingre in IngredientCollection)
            {
                if (ingre.DuocChon == true)
                {
                    if (ingre.DinhLuong <= 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private void RefreshIngredients()
        {
            foreach (Kho item in IngredientCollection)
            {
                item.DuocChon = false;
                item.DinhLuong = 0;
            }
        }
        #endregion
    }
}
