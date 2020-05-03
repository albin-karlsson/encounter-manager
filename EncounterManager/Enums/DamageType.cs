// Albin Karlsson 2019-01-12

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EncounterManager
{
    public enum DamageType
    {
        Acid,
        Bludgeoning,
        Cold,
        Fire,
        Force,
        Lightning,
        [Display(Name = "Magic Bludgeoning")]
        MagicBludgeoning,
        [Display(Name = "Magic Piercing")]
        MagicPiercing,
        [Display(Name = "Magic Slashing")]
        MagicSlashing,
        Necrotic,
        Piercing,
        Poison,
        Psychic,
        Radiant,
        Slashing,
        Thunder,

    }
}
