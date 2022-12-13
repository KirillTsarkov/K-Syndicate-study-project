using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Data;
using CodeBase.Logic.EnemySpawners;
using CodeBase.Services.PersistentProgress;
using UnityEngine.Purchasing;

namespace CodeBase.Services.IAP
{
  public class IAPService : IIAPService
  {
    private readonly IAPProvider _iapProvider;
    private readonly IPersistentProgressService _progressService;
    private List<SpawnPoint> _spawnPoints;

    public bool IsInitialized => _iapProvider.IsInitialized;
    public event Action Initialized;

    public IAPService(IAPProvider iapProvider, IPersistentProgressService progressService)
    {
      _iapProvider = iapProvider;
      _progressService = progressService;
    }

    public void Initialize()
    {
      _iapProvider.Initialize(this);
      _iapProvider.Initialized += () => Initialized?.Invoke();
    }

    public List<ProductDescription> Products() =>
      ProductDescriptions().ToList();

    public void StartPurchase(string productId) =>
      _iapProvider.StartPurchase(productId);

    public PurchaseProcessingResult ProcessPurchase(Product purchaseProduct)
    {
      ProductConfig productConfig = _iapProvider.Configs[purchaseProduct.definition.id];
     
      switch (productConfig.ItemType)
      {
        case ItemType.Skull:
          _progressService.Progress.WorldData.LootData.Add(productConfig.Quantity);
          _progressService.Progress.PurchaseData.AddPurchase(purchaseProduct.definition.id);
          break;
        case ItemType.RespawnEnemyPotion:
          foreach (SpawnPoint spawnPoint in _spawnPoints) spawnPoint.Respawn();
          break;
      }

      return PurchaseProcessingResult.Complete;
    }

    public void GetSpawners(List<SpawnPoint> spawnPoints) =>
      _spawnPoints = spawnPoints;

    private IEnumerable<ProductDescription> ProductDescriptions()
    {
      PurchaseData purchaseData = _progressService.Progress.PurchaseData;
      foreach (string productsId in _iapProvider.Products.Keys)
      {
        ProductConfig productConfig = _iapProvider.Configs[productsId];
        Product product = _iapProvider.Products[productsId];

        BoughtIAP boughtIAP = purchaseData.BoughtIAPs.Find(x => x.IAPid == productsId);
        if (ProductBoughtOut(boughtIAP, productConfig))
          continue;
        
        yield return new ProductDescription()
        {
          Id = productsId,
          Product = product,
          Config = productConfig,
          AvailablePurchasesLeft = boughtIAP != null
            ? productConfig.MaxPurchaseCount - boughtIAP.Count
            : productConfig.MaxPurchaseCount
        };
      }
    }

    private static bool ProductBoughtOut(BoughtIAP boughtIAP, ProductConfig productConfig) =>
      boughtIAP != null && boughtIAP.Count >= productConfig.MaxPurchaseCount;
  }
}