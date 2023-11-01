namespace Shop
{
    public interface IShop
    {
        public bool AIShopRequest(EHorse type, int num);
        public void NextRound();
    }
}