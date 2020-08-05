using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Image = System.Windows.Controls.Image;

namespace EngineeringProjectApp
{
    class Item
    {
        private ItemType itemType;
        private double x;
        private double y;
        private Image image;

        public Item(ItemType itemType)
        {
            this.itemType = itemType;
        }

        public Item(ItemType itemType, double x, double y, Image image) {
            this.itemType = itemType;
            this.x = x;
            this.y = y;
            this.image = image;
        }

        public ItemType getItemType() { return this.itemType; }
        public double getX() { return this.x; }
        public double getY() { return this.y; }
        public Image getImage() { return this.image; }
        public void setItemType(ItemType itemType) { this.itemType = itemType; }
        public void setX(float x) { this.x = x; }
        public void setY(float y) { this.y = y; }
        public void setImage(Image image) { this.image = image; }
    }
}
