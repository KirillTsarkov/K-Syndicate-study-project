using System.Threading.Tasks;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Services.IAP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Windows.Shop
{
  public class ShopItem : MonoBehaviour
  {
    public Button BuyItemButton;
    public TextMeshProUGUI PriceText;
    public TextMeshProUGUI QuantityText;
    public TextMeshProUGUI AvailableItemsLeftText;
    public Image Icon;
    
    private ProductDescription _productDescription;
    private IIAPService _iapService;
    private IAssetProvider _assetProvider;

    public void Construct(ProductDescription productDescription, IIAPService iapService, IAssetProvider assetProvider)
    {
      _productDescription = productDescription;
      
      _iapService = iapService;
      _assetProvider = assetProvider;
    }

    public async void Initialize()
    {
      BuyItemButton.onClick.AddListener(OnBuyItemClick);
      PriceText.text = _productDescription.Config.Price;
      QuantityText.text = _productDescription.Config.Quantity.ToString();
      AvailableItemsLeftText.text = _productDescription.AvailablePurchasesLeft.ToString();
      Icon.sprite = await _assetProvider.Load<Sprite>(_productDescription.Config.Icon);
    }

    private void OnBuyItemClick() =>
      _iapService.StartPurchase(_productDescription.Id);
  }
}