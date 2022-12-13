using System;
using System.Collections.Generic;
using CodeBase.Logic.EnemySpawners;

namespace CodeBase.Services.IAP
{
  public interface IIAPService : IService
  {
    bool IsInitialized { get; }
    event Action Initialized;
    void Initialize();
    List<ProductDescription> Products();
    void StartPurchase(string productId);
    void GetSpawners(List<SpawnPoint> spawnPoints);
  }
}