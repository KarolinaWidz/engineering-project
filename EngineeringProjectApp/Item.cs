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
        private double startX;
        private double startY;
        private double x;
        private double y;
        private Image image;
        private Position actualPosition;
        private Position targetPosition;
        private Position previousPosition;
        private bool correctPosition;

        public Item(ItemType itemType, Position targetPosition)
        {
            this.itemType = itemType;
            this.actualPosition = Position.OTHER;
            this.targetPosition = targetPosition;
            this.correctPosition = false;
        }

        public ItemType getItemType() { return this.itemType; }
        public double getX() { return this.x; }
        public double getY() { return this.y; }
        public double getStartX() { return this.startX; }
        public double getStartY() { return this.startY; }
        public Image getImage() { return this.image; }
        public Position getActualPosition() { return this.actualPosition; }
        public Position getTargetPosition() { return this.targetPosition; }
        public Position getPreviousPosition() { return this.previousPosition; }
        public bool getCorrectPosition() { return this.correctPosition; }
        public void setItemType(ItemType itemType) { this.itemType = itemType; }
        public void setX(double x) { this.x = x; }
        public void setY(double y) { this.y = y; }
        public void setStartX(double startX) { this.startX = startX; }
        public void setStartY(double startY) { this.startY = startY; }
        public void setImage(Image image) { this.image = image; }
        public void setActualPosition(Position actualPosition) { this.actualPosition = actualPosition; }
        public void setPreviousPosition(Position previousPosition) { this.previousPosition = previousPosition; }
        public void setCorrectPostionFlag(bool correctPositionFlag) { this.correctPosition = correctPositionFlag; }

    }
}
