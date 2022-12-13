using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Data;
using UnityEngine;
using UnityEngine.Purchasing;

namespace CodeBase.Services.IAP
{
  public class IAPProvider : IStoreListener
  {
    private const string IAPConfigsPath = "IAP/products";

    public Dictionary<string, ProductConfig> Configs { get; private set; }
    public Dictionary<string, Product> Products { get; private set; }
    private IStoreController _controller;
    private IExtensionProvider _extension;
    private IAPService _iapService;

    public bool IsInitialized => _extension != null && _controller != null;
    public event Action Initialized;

    public void Initialize(IAPService iapService)
    {
      _iapService = iapService;

      Configs = new Dictionary<string, ProductConfig>();
      Products = new Dictionary<string, Product>();

      Load();

      ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
      foreach (ProductConfig config in Configs.Values)
        builder.AddProduct(config.Id, config.ProductType);

      UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
      _controller = controller;
      _extension = extensions;

      foreach (Product product in _controller.products.all)
        Products.Add(product.definition.id, product);

      Initialized?.Invoke();
      Debug.Log("UnityPurchasing inialization success!");
    }

    public void StartPurchase(string productId) =>
      _controller.InitiatePurchase(productId);

    public void OnInitializeFailed(InitializationFailureReason error)
    {
      Debug.Log($"UnityPurchasing OnInitializeFailed {error}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
      Debug.Log($"UnityPurchasing success {purchaseEvent.purchasedProduct.definition.id}");
      return _iapService.ProcessPurchase(purchaseEvent.purchasedProduct);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) =>
      Debug.LogError(
        $"Product {product.definition.id} purchase failed, PurchaseFailureReason {failureReason}, transaction id {product.transactionID}");

    private void Load() =>
      Configs = Resources.Load<TextAsset>(IAPConfigsPath).text.ToDeserialized<ProductConfigWrapper>().Configs
        .ToDictionary(x => x.Id, x => x);
  }
}