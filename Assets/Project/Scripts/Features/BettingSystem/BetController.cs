using System.Collections.Generic;
using Game.Core.Area;
using RouletteGame.Interfaces;
using UnityEngine;

namespace RouletteGame.Models
{
    // public class BetController : MonoBehaviour
    // {
    //     [SerializeField] AreaSelector areaSelector;
    //     ChipManager _chipManager;
    //     IBettingSystem _bettingSystem;
    //     public void Initialize(ChipManager chipManager, IBettingSystem bettingSystem)
    //     {
    //         _chipManager = chipManager;
    //         _bettingSystem = bettingSystem;
    //         areaSelector.OnSelectArea += AreaSelectorOnSelectArea;
    //         _bettingSystem.OnBetAdded += BettingSystemOnOnBetAdded;
    //     }
    //
    //     private void OnDisable()
    //     {
    //         areaSelector.OnSelectArea -= AreaSelectorOnSelectArea;
    //         _bettingSystem.OnBetAdded -= BettingSystemOnOnBetAdded;
    //
    //     }
    //     private void BettingSystemOnOnBetAdded(IBet obj)
    //     {
    //         //unselect recent winning number after bet
    //         HoverWinningBetNumber(-1);
    //     }
    //
    //     public void HoverWinningBetNumber(int betNumber)
    //     {
    //         areaSelector.SetHoveredArea(betNumber);
    //     }
    //     private void AreaSelectorOnSelectArea(Area area)
    //     {
    //         if(area==null)
    //             return;
    //         var bet = SignalBetFactory.CreateBetFromSignal(area.GetID(), _chipManager.SelectedChipValue);
    //         if (bet != null)
    //         {
    //             areaSelector.SetCoveredAreaNumbers(new List<int>(bet.CoveredNumbers));
    //             _bettingSystem.AddBet(bet);
    //         }
    //     }
    // }
}