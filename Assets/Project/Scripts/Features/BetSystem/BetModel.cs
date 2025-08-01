using System.Collections.Generic;
using UnityEngine;

namespace Game.BetSystem
{
    public class BetModel : MonoBehaviour
    {
        [SerializeField] private List<BetChips> betChips = new();
    }
}