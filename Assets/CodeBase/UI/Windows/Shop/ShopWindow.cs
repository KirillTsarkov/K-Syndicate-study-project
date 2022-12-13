using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Services.Ads;
using CodeBase.Services.IAP;
using CodeBase.Services.PersistentProgress;
using TMPro;

namespace CodeBase.UI.Windows.Shop
{
  public class ShopWindow : WindowBase
  {
    public TextMeshProUGUI SkullText;
    public RewardedAdItem AdItem;
    public ShopItemsContainer ShopItemsContainer;

    public void Construct(IAdsService adsService,
      IPersistentProgressService progressService,
      IIAPService iapService,
      IAssetProvider assetProvider)
    {
      base.Construct(progressService);
      AdItem.Construct(adsService, progressService);
      ShopItemsContainer.Construct(iapService, progressService, assetProvider);
    }

    protected override void Initialize()
    {
      AdItem.Initialize();
      ShopItemsContainer.Initialize();
      RefreshSkullText();
    }

    protected override void SubscribeUpdates()
    {
      AdItem.Subscribe();
      ShopItemsContainer.Subscribe();
      Progress.WorldData.LootData.Changed += RefreshSkullText;
    }

    protected override void Cleanup()
    {
      base.Cleanup();
      AdItem.Cleanup();
      ShopItemsContainer.Cleanup();
      Progress.WorldData.LootData.Changed -= RefreshSkullText;
    }

    private void RefreshSkullText() =>
      SkullText.text = Progress.WorldData.LootData.Collected.ToString();
  }
}