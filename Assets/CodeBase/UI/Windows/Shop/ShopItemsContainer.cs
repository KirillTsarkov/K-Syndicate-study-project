using System.Collections.Generic;
using System.Threading.Tasks;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Services.IAP;
using CodeBase.Services.PersistentProgress;
using UnityEngine;

namespace CodeBase.UI.Windows.Shop
{
  public class ShopItemsContainer : MonoBehaviour
  {
    private const string ShopItemPath = "ShopItem";

    public GameObject[] shopUnavailableItems;
    public Transform Parent;

    private IIAPService _iapService;
    private IPersistentProgressService _progressService;
    private IAssetProvider _assetProvider;
    private readonly List<GameObject> _shopItems = new List<GameObject>();

    public void Construct(IIAPService iapService, IPersistentProgressService progressService,
      IAssetProvider assetProvider)
    {
      _progressService = progressService;
      _iapService = iapService;
      _assetProvider = assetProvider;
    }

    public void Initialize() =>
      RefreshAvailableItems();

    public void Subscribe()
    {
      _iapService.Initialized += RefreshAvailableItems;
      _progressService.Progress.PurchaseData.Changed += RefreshAvailableItems;
    }

    public void Cleanup()
    {
      _iapService.Initialized -= RefreshAvailableItems;
      _progressService.Progress.PurchaseData.Changed -= RefreshAvailableItems;
    }

    private async void RefreshAvailableItems()
    {
      UpdateShopUnavailableItems();

      if (!_iapService.IsInitialized)
        return;

      ClearShopItems();

      await FillShopItems();
    }

    private void ClearShopItems()
    {
      foreach (GameObject shopItem in _shopItems)
        Destroy(shopItem);
    }

    private async Task FillShopItems()
    {
      foreach (ProductDescription productDescription in _iapService.Products())
      {
        GameObject shopItemObject = await _assetProvider.Instantiate(ShopItemPath, Parent);
        ShopItem shopItem = shopItemObject.GetComponent<ShopItem>();
        shopItem.Construct(productDescription, _iapService, _assetProvider);
        shopItem.Initialize();
        _shopItems.Add(shopItemObject);
      }
    }

    private void UpdateShopUnavailableItems()
    {
      foreach (GameObject shopUnavailableItem in shopUnavailableItems)
        shopUnavailableItem.SetActive(!_iapService.IsInitialized);
    }
  }
}