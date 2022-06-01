using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class LaborAndEquipmentUserControl : UserControl
    {
        public double qtyDouble, daysDouble, hrsDouble, priceDouble, totalPrice;
        public int id;
        public string name;

        public LaborAndEquipmentUserControl(string item, string qty, object price, int id)
        {
            InitializeComponent();
            qtyDouble = double.Parse(qty, System.Globalization.CultureInfo.InvariantCulture);
            daysDouble = double.Parse(days, System.Globalization.CultureInfo.InvariantCulture);
            hrsDouble = double.Parse(hrs, System.Globalization.CultureInfo.InvariantCulture);
            priceDouble = double.Parse(price.ToString(), System.Globalization.CultureInfo.InvariantCulture);
            name = item;
            this.id = id;

            Compute();
        }

        private void Compute()
        {
            totalPrice = qtyDouble * hrsDouble * daysDouble * priceDouble;
            this.price = "₱" + totalPrice.ToString();
        }

        private void laq_hrs_bx_TextChanged(object sender, EventArgs e)
        {
            hrsDouble = double.Parse(hrs, System.Globalization.CultureInfo.InvariantCulture);
            Compute();
        }

        private void laq_days_bx_TextChanged(object sender, EventArgs e)
        {
            daysDouble = double.Parse(days, System.Globalization.CultureInfo.InvariantCulture);
            Compute();
        }

        public string laq
        {
            get
            {
                return laq_Label.Text;
            }
            set
            {
                laq_Label.Text = value;
            }
        }

        public string hrs
        {
            get
            {
                return laq_hrs_bx.Text;
            }
            set 
            { 
                laq_hrs_bx.Text = value; 
            }
        }

        public string days
        {
            get
            {
                return laq_days_bx.Text;
            }
            set
            {
                laq_days_bx.Text = value;
            }
        }

        public string price
        {
            get
            {
                return price_Label.Text;
            }
            set
            {
                price_Label.Text = value;
            }
        }
    }
}
