using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace ListViewExample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void cmdFillList_Click(object sender, EventArgs e)
        {
            FillList();
        }

        private void FillList()
        {
            listView.Items.Clear();

            // Create groups if needed. They only
            // appear is ShowGroups is true.
            if (listView.Groups.Count == 0)
            {
                DataTable dtGroups = StoreDB.GetCategories();
                foreach (DataRow dr in dtGroups.Rows)
                {
                    listView.Groups.Add(dr["CategoryID"].ToString(),
                        dr["CategoryName"].ToString());
                }
            }
            // Turn groups on or off.
            listView.ShowGroups = chkGroups.Checked;

            DataTable dtProducts = StoreDB.GetProducts();

            // Suspending automatic refreshes as items are added/removed.
            listView.BeginUpdate();

            listView.SmallImageList = imagesSmall;
            listView.LargeImageList = imagesLarge;
            foreach (DataRow dr in dtProducts.Rows)
            {
                ListViewItem listItem = new ListViewItem(dr["ModelName"].ToString());
                listItem.ImageKey =  "D" + textBoxCaption.Text;
                //listItem.ImageIndex = 1;

                // Put it in the appropriate group.
                // (Only has an effect if ShowGroups is true.)
                listItem.Group = listView.Groups[dr["CategoryID"].ToString()];

                // Add sub-items for Details view.
                listItem.SubItems.Add(dr["ProductID"].ToString());
                listItem.SubItems.Add(dr["Description"].ToString());

                listView.Items.Add(listItem);
            }

            // Add column headers for Details view
            // if they haven't been added before.
            if (listView.Columns.Count == 0)
            {
                listView.Columns.Add("Product", 100, HorizontalAlignment.Left);
                listView.Columns.Add("ID", 100, HorizontalAlignment.Left);
                listView.Columns.Add("Description", 100, HorizontalAlignment.Left);
            }

            // Re-enable the display.
            listView.EndUpdate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView.View = View.Tile;
            listView.TileSize = new Size(300, 50);
            optLargeIcon.Tag = View.LargeIcon;
            optSmallIcon.Tag = View.SmallIcon;
            optDetails.Tag = View.Details;
            optList.Tag = View.List;
            optTile.Tag = View.Tile;
            //FillList();
            CreateIcons2();
        }

        private void NewView(object sender, System.EventArgs e)
        {
            // Set the current view mode based on the number in the tag value of the
            // selected radio button.

            listView.View = (View)(((Control)sender).Tag);

            // Display the current view style.
            this.Text = "Using View: " + listView.View.ToString();
        }

        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Check the current sort.
            ListViewItemComparer sorter = listView.ListViewItemSorter as ListViewItemComparer;

            // Specify an alphabetic sort based on the column that was clicked.
            if (sorter == null)
            {
                sorter = new ListViewItemComparer(e.Column);
                listView.ListViewItemSorter = sorter;
            }
            else
            {
                if (sorter.Column == e.Column && !sorter.Descending)
                {
                    // The list is already sorted on this column.
                    // Time to flip the sort.
                    sorter.Descending = true;
                    // Keep the ListView.Sorting property 
                    // synchronized, just for tidiness.
                    listView.Sorting = SortOrder.Descending;
                }
                else
                {
                    listView.Sorting = SortOrder.Ascending;
                    sorter.Descending = false;
                    sorter.Column = e.Column;
                }
            }

            // Perform the sort.
            listView.Sort();
        }

        private void cmdResizeColumns_Click(object sender, EventArgs e)
        {
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }


        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
                txtSelected.Text = listView.SelectedItems[0].SubItems[2].Text;
        }

        private void chkGroups_CheckedChanged(object sender, EventArgs e)
        {
            FillList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateIcons2();
        }

        private void CreateIcons2()
        {
            //this.imagesLarge.Images.Clear();
            //this.imagesSmall.Images.Clear();

            string key = "D" + textBoxCaption.Text;
            if (!this.imagesSmall.Images.ContainsKey(key))
            {
                this.imagesSmall.Images.Add(key, getIcon(textBoxCaption.Text, 16, 5));
            }
            if (!this.imagesLarge.Images.ContainsKey(key))
            {
                this.imagesLarge.Images.Add(key,getIcon(textBoxCaption.Text, 32, 8));
            }
            FillList();
        }

        private Image getIcon(string val, int size,int fontSize)
        {
            
            Bitmap newBitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;      // Horizontal Alignment
                stringFormat.LineAlignment = StringAlignment.Center;  // Vertical Alignment
                Rectangle rect = new Rectangle(0, 0, size, size);

                using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.White, Color.LightGray, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, rect);
                    g.DrawRectangle(Pens.Black, 0, 0, size - 1, size - 1);
                    g.TextContrast = 0;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                    g.DrawString(val, new Font("Tahona", fontSize, FontStyle.Bold), Brushes.Black, rect, stringFormat);
                }
            }
            return newBitmap;
        }

        //private void CreateIcons()
        //{
        //    //_cnt++;
        //    _cnt = 100;
        //    this.pictureBox1.Image = null;
        //    this.pictureBox2.Image = null;

        //    Bitmap newBitmap1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        //    using (Graphics g1 = Graphics.FromImage(newBitmap1))
        //    {
        //        StringFormat stringFormat = new StringFormat();
        //        stringFormat.Alignment = StringAlignment.Center;      // Horizontal Alignment
        //        stringFormat.LineAlignment = StringAlignment.Center;  // Vertical Alignment
        //        Rectangle rect1 = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);

        //        using (LinearGradientBrush brush = new LinearGradientBrush(rect1, Color.White, Color.LightGray, LinearGradientMode.Vertical))
        //        {
        //            g1.FillRectangle(brush, rect1);
        //            g1.DrawRectangle(Pens.Black, 0, 0, pictureBox1.Width - 1, pictureBox1.Height - 1);
        //            g1.TextContrast = 0;
        //            g1.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        //            //g1.DrawString(_cnt.ToString() + "%", new Font("Tahona", 8, FontStyle.Bold), Brushes.Black, rect1, stringFormat);
        //            g1.DrawString(_cnt.ToString() + "%", new Font("Tahona", 8, FontStyle.Bold), Brushes.Black, rect1, stringFormat);
        //        }

        //    }

        //    this.imagesLarge.Images.Clear();
        //    pictureBox4.Image = newBitmap1;
        //    this.imagesLarge.Images.Add( this.pictureBox4.Image);


        //    Bitmap newBitmap2 = new Bitmap(pictureBox3.Width, pictureBox3.Height);
        //    using (Graphics g2 = Graphics.FromImage(newBitmap2))
        //    {
        //        g2.FillRectangle(Brushes.LightGray, 0, 0, pictureBox3.Width, pictureBox3.Height);
        //        g2.TextContrast = 0;
        //        g2.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        //        g2.DrawString(_cnt.ToString(), new Font("Tahona", 6), Brushes.Black, new Point(0, 5));
        //    }

        //    this.imagesSmall.Images.Clear();
        //    pictureBox4.Image = newBitmap2;
        //    this.imagesSmall.Images.Add(this.pictureBox4.Image);



        //    FillList();
        //}

    }
}