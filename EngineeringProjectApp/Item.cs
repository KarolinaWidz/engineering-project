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

        public ItemType GetItemType() { return this.itemType; }
        public double GetX() { return this.x; }
        public double GetY() { return this.y; }
        public double GetStartX() { return this.startX; }
        public double GetStartY() { return this.startY; }
        public Image GetImage() { return this.image; }
        public Position GetActualPosition() { return this.actualPosition; }
        public Position GetTargetPosition() { return this.targetPosition; }
        public Position GetPreviousPosition() { return this.previousPosition; }
        public bool GetCorrectPosition() { return this.correctPosition; }
        public void SetItemType(ItemType itemType) { this.itemType = itemType; }
        public void SetX(double x) { this.x = x; }
        public void SetY(double y) { this.y = y; }
        public void SetStartX(double startX) { this.startX = startX; }
        public void SetStartY(double startY) { this.startY = startY; }
        public void SetImage(Image image) { this.image = image; }
        public void SetActualPosition(Position actualPosition) { this.actualPosition = actualPosition; }
        public void SetPreviousPosition(Position previousPosition) { this.previousPosition = previousPosition; }
        public void SetCorrectPostionFlag(bool correctPositionFlag) { this.correctPosition = correctPositionFlag; }

    }
}
