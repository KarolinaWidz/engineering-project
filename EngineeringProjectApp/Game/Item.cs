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
        private readonly Position targetPosition;
        private Position previousPosition;
        private bool correctPosition;

        public Item(ItemType itemType, Position targetPosition)
        {
            this.itemType = itemType;
            this.actualPosition = Position.OTHER;
            this.targetPosition = targetPosition;
            this.correctPosition = false;
        }

        public ItemType GetItemType() { return itemType; }
        public double GetX() { return x; }
        public double GetY() { return y; }
        public double GetStartX() { return startX; }
        public double GetStartY() { return startY; }
        public Image GetImage() { return image; }
        public Position GetActualPosition() { return actualPosition; }
        public Position GetTargetPosition() { return targetPosition; }
        public Position GetPreviousPosition() { return previousPosition; }
        public bool GetCorrectPosition() { return correctPosition; }
        public void SetItemType(ItemType itemType) { this.itemType = itemType; }
        public void SetX(double x) { this.x = x; }
        public void SetY(double y) { this.y = y; }
        public void SetStartX(double startX) { this.startX = startX; }
        public void SetStartY(double startY) { this.startY = startY; }
        public void SetImage(Image image) { this.image = image; }
        public void SetActualPosition(Position actualPosition) { this.actualPosition = actualPosition; }
        public void SetPreviousPosition(Position previousPosition) { this.previousPosition = previousPosition; }
        public void SetCorrectPostionFlag(bool correctPositionFlag) { correctPosition = correctPositionFlag; }

    }
}
