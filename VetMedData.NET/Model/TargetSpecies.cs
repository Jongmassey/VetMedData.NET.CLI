using System;
using System.Collections.Generic;
using System.Linq;

namespace VetMedData.NET.Model
{
    [Serializable]
    public class TargetSpecies
    {
        public string CanonicalName { get; set; }
        public string[] Synonyms { get; set; }
        public string[] Names =>
            Synonyms == null ? new[] { CanonicalName } : new[] { CanonicalName }.Union(Synonyms).ToArray();
        
        public static IEnumerable<TargetSpecies> All { get; } = new[]
        {
            new TargetSpecies {CanonicalName = "all animals"},
            new TargetSpecies {CanonicalName = "avian spp"},
            new TargetSpecies {CanonicalName = "badgers"},
            new TargetSpecies {CanonicalName = "bearded dragons"},
            new TargetSpecies {CanonicalName = "bees"},
            new TargetSpecies {CanonicalName = "budgerigar"},
            new TargetSpecies {CanonicalName = "cage birds",
                Synonyms = new []{
                    "budgerigar",
                    "canaries"
                }},
            new TargetSpecies {CanonicalName = "canaries"},
            new TargetSpecies {CanonicalName = "cats"},
            new TargetSpecies {CanonicalName = "cattle",
                Synonyms = new []
                {
                    "cows"
                    , "calves"
                }},
            new TargetSpecies {CanonicalName = "chickens"},
            new TargetSpecies {CanonicalName = "chinchilla"},
            new TargetSpecies {CanonicalName = "deer"},
            new TargetSpecies {CanonicalName = "dogs"},
            new TargetSpecies {CanonicalName = "donkeys"},
            new TargetSpecies {CanonicalName = "ducks"},
            new TargetSpecies {CanonicalName = "exotic animals"},
            new TargetSpecies {CanonicalName = "ferrets"},
            new TargetSpecies {CanonicalName = "fish"},
            new TargetSpecies {CanonicalName = "frogs"},
            new TargetSpecies {CanonicalName = "game birds",
                Synonyms = new []
                {
                    "partridge",
                    "pigeons",
                    "quails",
                    "pheasants"
                }},
            new TargetSpecies {CanonicalName = "geese"},
            new TargetSpecies {CanonicalName = "gerbil"},
            new TargetSpecies {CanonicalName = "goats"},
            new TargetSpecies {CanonicalName = "guinea pigs"},
            new TargetSpecies {CanonicalName = "hamsters"},
            new TargetSpecies {CanonicalName = "hares"},
            new TargetSpecies {CanonicalName = "horses",
                Synonyms = new []
                {
                    "foals"
                }},
            new TargetSpecies {CanonicalName = "lizards"},
            new TargetSpecies {CanonicalName = "mice"},
            new TargetSpecies {CanonicalName = "mink"},
            new TargetSpecies {CanonicalName = "ornamental birds"},
            new TargetSpecies {CanonicalName = "ornamental fish"},
            new TargetSpecies {CanonicalName = "partridge"},
            new TargetSpecies {CanonicalName = "pheasants"},
            new TargetSpecies {CanonicalName = "pigeons"},
            new TargetSpecies {CanonicalName = "pigs"},
            new TargetSpecies {CanonicalName = "polecats"},
            new TargetSpecies {CanonicalName = "poultry",
                Synonyms = new []
                {
                    "chickens",
                    "turkeys",
                    "ducks",
                    "geese",
                    "chinchilla"
                }},
            new TargetSpecies {CanonicalName = "quails"},
            new TargetSpecies {CanonicalName = "rabbits"},
            new TargetSpecies {CanonicalName = "rats"},
            new TargetSpecies {CanonicalName = "red foxes"},
            new TargetSpecies {CanonicalName = "reptiles"},
            new TargetSpecies {CanonicalName = "rodents",
                Synonyms = new []
                {
                    "rats",
                    "mice",
                    "hamsters",
                    "guinea pigs",
                    "gerbil"
                }},
            new TargetSpecies {CanonicalName = "salmon",
                Synonyms = new []
                {
                    "salmon (atlantic)"
                }},
            new TargetSpecies {CanonicalName = "salmon (atlantic)"},
            new TargetSpecies {CanonicalName = "sheep"},
            new TargetSpecies {CanonicalName = "small mammals"},
            new TargetSpecies {CanonicalName = "snakes"},
            new TargetSpecies {CanonicalName = "sub human primates"},
            new TargetSpecies {CanonicalName = "tortoises"},
            new TargetSpecies {CanonicalName = "trout",
                Synonyms = new []
                {
                    "trout (rainbow)"
                }},
            new TargetSpecies {CanonicalName = "trout (rainbow)"},
            new TargetSpecies {CanonicalName = "turkeys"},
        };

        public static IEnumerable<TargetSpecies> Find(string instr)
        {
            return All.Where(a =>
                a.Names.Select(
                    n => n.ToLowerInvariant().Split(' ').Intersect(instr.ToLowerInvariant().Split(' ')).Any()).Any());
        }
    }
}
