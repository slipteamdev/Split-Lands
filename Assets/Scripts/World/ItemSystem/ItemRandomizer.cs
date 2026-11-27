using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemRandomizer : MonoBehaviour
{
    [Serializable]
    public struct TypeWeight
    {
        public ItemType type;
        [Range(0f, 100f)] public float weight;
    }

    [Serializable]
    public struct TierWeight
    {
        public ItemTier tier;
        [Range(0f, 100f)] public float weight;
    }

    // Lista de prefabs disponibles para generacion
    [Header("Item List")]
    [SerializeField] private GameObject[] itemPrefabs;

    // Selector de probabilidades para cada categoria de objeto
    [Header("Type Weight (probability)")]
    [SerializeField]
    private TypeWeight[] typeProbabilities = new TypeWeight[]
    {
        new TypeWeight { type = ItemType.Draw, weight = 30f },
        new TypeWeight { type = ItemType.Damage, weight = 25f },
        new TypeWeight { type = ItemType.Buff, weight = 20f },
        new TypeWeight { type = ItemType.Debuff, weight = 15f },
        new TypeWeight { type = ItemType.Other, weight = 10f }
    };

    // Selector de probabilidades para cada rareza
    [Header("Tier Weight (probability)")]
    [SerializeField]
    private TierWeight[] tierProbabilities = new TierWeight[]
    {
        new TierWeight { tier = ItemTier.Common, weight = 60f },
        new TierWeight { tier = ItemTier.Uncommon, weight = 30f },
        new TierWeight { tier = ItemTier.Rare, weight = 10f }
    };

    // Diccionario anidado. Este generador funciona de la siguiente manera: Se elige una categoria de objeto de
    // manera aleatoria, y despues se elige una rareza de manera aleatoria. Finalmente se elige un objeto de entre los
    // que tengan esa categoria y rareza.
    // Cuando le das una categoria a este diccionario anidado, te da otro diccionario. Si a ese diccionario le das la
    // rareza, te devuelve una lista con los objetos que cumplen ambas condiciones.
    private Dictionary<ItemType, Dictionary<ItemTier, List<GameObject>>> catalogCache;

    // Listas auxiliares, sirven para indicar las categorias y rarezas disponibles (hay combinaciones de categoria y rareza
    // que no existiran, especialmente ahora que aun faltan muchos objetos, estas listas estan para evitar que esas combinaciones
    // faltantes supongan un problema)
    private List<ItemType> availableTypesCache;
    private Dictionary<ItemType, List<ItemTier>> availableTiersByTypeCache;

    // Variables para el calculo de probabilidad en funcion del peso
    private float totalValidTypeWeight = 0f;
    private Dictionary<ItemType, float> totalValidTierWeightByType;

    // bool para comprobar si se han inicializado las listas antes de generar un objeto
    private bool isInitialized = false;

    // Listas reutilizables para filtrar
    private readonly List<ItemType> tempValidTypes = new List<ItemType>();
    private readonly List<ItemTier> tempValidTiers = new List<ItemTier>();

    private void Awake()
    {
        BuildCache();
    }

    #region Inicializacion
    // Recorre la lista de objetos para separar por categorias y rarezas, guardando los datos para no repetir
    // los calculos cada vez que se genera un objeto
    public void BuildCache()
    {
        catalogCache = new Dictionary<ItemType, Dictionary<ItemTier, List<GameObject>>>();
        availableTypesCache = new List<ItemType>();
        availableTiersByTypeCache = new Dictionary<ItemType, List<ItemTier>>();
        totalValidTierWeightByType = new Dictionary<ItemType, float>();
        totalValidTypeWeight = 0f;

        if (itemPrefabs == null || itemPrefabs.Length == 0)
        {
            Debug.LogWarning("[ItemRandomizer] No hay prefabs asignados en el inspector.");
            return;
        }

        // Clasificar cada prefab en la estructura anidada
        foreach (GameObject prefab in itemPrefabs)
        {
            if (prefab == null) continue;

            if (!prefab.TryGetComponent<Item>(out Item itemComp))
            {
                Debug.LogWarning($"[ItemRandomizer] El prefab '{prefab.name}' no tiene un componente Item y será ignorado.");
                continue;
            }

            ItemType type = itemComp.type;
            ItemTier tier = itemComp.tier;

            // Si la categoria aun no existe en la cache, la inicializamos
            if (!catalogCache.TryGetValue(type, out var tierDict))
            {
                tierDict = new Dictionary<ItemTier, List<GameObject>>();
                catalogCache[type] = tierDict;
                availableTypesCache.Add(type);
                availableTiersByTypeCache[type] = new List<ItemTier>();
            }

            // Si la rareza aun no existe dentro de esta categoria, la inicializamos
            if (!tierDict.TryGetValue(tier, out var prefabList))
            {
                prefabList = new List<GameObject>();
                tierDict[tier] = prefabList;
                availableTiersByTypeCache[type].Add(tier);
            }

            prefabList.Add(prefab);
        }

        if (catalogCache.Count == 0) return;

        // Precalcular la suma de pesos de las categorias que si tienen objetos
        foreach (var entry in typeProbabilities)
        {
            if (catalogCache.ContainsKey(entry.type) && entry.weight > 0f)
            {
                totalValidTypeWeight += entry.weight;
            }
        }

        // Precalcular la suma de pesos de rarezas disponibles para cada categoría
        foreach (ItemType type in availableTypesCache)
        {
            float tierWeightSum = 0f;
            var tierDict = catalogCache[type];

            foreach (var entry in tierProbabilities)
            {
                if (tierDict.ContainsKey(entry.tier) && entry.weight > 0f)
                {
                    tierWeightSum += entry.weight;
                }
            }
            totalValidTierWeightByType[type] = tierWeightSum;
        }

        isInitialized = true;
    }

    #endregion

    // Devuelve un prefab aleatorio segun las probabilidades elegidas
    public GameObject GetRandomItemPrefab(ItemType[] allowedTypes = null, ItemTier[] allowedTiers = null)
    {
        if (!isInitialized) BuildCache();
        if (catalogCache == null || catalogCache.Count == 0) return null;

        bool hasTypeFilter = allowedTypes != null && allowedTypes.Length > 0;
        bool hasTierFilter = allowedTiers != null && allowedTiers.Length > 0;

        // Camino normal sin filtros
        if (!hasTypeFilter && !hasTierFilter)
        {
            // Elegir Categoria
            ItemType selectedType = PickCachedType();
            var tierDict = catalogCache[selectedType];

            // Elegir Rareza dentro de esa categoria
            ItemTier selectedTier = PickCachedTier(selectedType, tierDict);
            List<GameObject> finalCandidates = tierDict[selectedTier];

            // Elegir objeto final
            return finalCandidates[UnityEngine.Random.Range(0, finalCandidates.Count)];
        }

        // Camino con filtros
        // Buscar categorias que cumplan el filtro Y que tengan al menos una rareza permitida
        tempValidTypes.Clear();
        for (int i = 0; i < availableTypesCache.Count; i++)
        {
            ItemType type = availableTypesCache[i];
            if (hasTypeFilter && !ContainsType(allowedTypes, type)) continue;
            if (hasTierFilter && !HasAnyAllowedTier(type, allowedTiers)) continue;

            tempValidTypes.Add(type);
        }

        if (tempValidTypes.Count == 0)
        {
            Debug.LogWarning("[ItemRandomizer] No hay ningun prefab que cumpla la combinacion de filtros solicitada.");
            return GetRandomItemPrefab(); // Devuelve objeto aleatorio sin filtros en ese caso
        }

        // Se elige una categoria de entre las permitidas
        ItemType filteredType = PickFilteredType(tempValidTypes);
        var filteredTierDict = catalogCache[filteredType];

        // Buscar todas las rarezas validas dentro de la categoria elegida
        tempValidTiers.Clear();
        var availableTiers = availableTiersByTypeCache[filteredType];
        for (int i = 0; i < availableTiers.Count; i++)
        {
            ItemTier tier = availableTiers[i];
            if (!hasTierFilter || ContainsTier(allowedTiers, tier))
            {
                tempValidTiers.Add(tier);
            }
        }

        // Se elige una rareza de entre las permitidas
        ItemTier filteredTier = PickFilteredTier(tempValidTiers);

        // Elegir objeto final
        List<GameObject> candidates = filteredTierDict[filteredTier];
        return candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }

    #region Metodos auxiliares
    // Metodo que recibe una categoria de objeto y las tiers usadas como filtro.
    // Devuelve si dicha categoria tiene algun objeto de esas rarezas
    private bool HasAnyAllowedTier(ItemType type, ItemTier[] allowedTiers)
    {
        var tiers = availableTiersByTypeCache[type]; // Esto devuelve las rarezas que tienen los objetos de la categoria
        for (int i = 0; i < tiers.Count; i++)
        {
            if (ContainsTier(allowedTiers, tiers[i])) return true;
        }
        return false;
    }

    // Metodo que comprueba si el tipo especificado se encuentra dentro de los tipos permitidos
    private bool ContainsType(ItemType[] array, ItemType target)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == target) return true;
        }
        return false;
    }

    // Metodo que comprueba si la rareza especificada se encuentra dentro de las rarezas permitidas
    private bool ContainsTier(ItemTier[] array, ItemTier target)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == target) return true;
        }
        return false;
    }

    // Elige categoria aleatoria en funcion de los pesos
    private ItemType PickCachedType()
    {
        if (totalValidTypeWeight <= 0f) // Si nada tiene peso, da algo 100% aleatorio
        {
            return availableTypesCache[UnityEngine.Random.Range(0, availableTypesCache.Count)];
        }

        float randomPoint = UnityEngine.Random.Range(0f, totalValidTypeWeight);
        float currentSum = 0f;

        for (int i = 0; i < typeProbabilities.Length; i++)
        {
            var entry = typeProbabilities[i];
            if (catalogCache.ContainsKey(entry.type) && entry.weight > 0f)
            {
                currentSum += entry.weight;
                if (randomPoint <= currentSum)
                {
                    return entry.type;
                }
            }
        }

        return availableTypesCache[0];
    }

    // Variacion del metodo superior utilizado al buscar objetos con filtro
    private ItemType PickFilteredType(List<ItemType> validTypes)
    {
        if (validTypes.Count == 1) return validTypes[0];

        float totalWeight = 0f;
        for (int i = 0; i < typeProbabilities.Length; i++)
        {
            var entry = typeProbabilities[i];
            if (entry.weight > 0f && validTypes.Contains(entry.type))
            {
                totalWeight += entry.weight;
            }
        }

        if (totalWeight <= 0f)
        {
            return validTypes[UnityEngine.Random.Range(0, validTypes.Count)];
        }

        float randomPoint = UnityEngine.Random.Range(0f, totalWeight);
        float currentSum = 0f;

        for (int i = 0; i < typeProbabilities.Length; i++)
        {
            var entry = typeProbabilities[i];
            if (entry.weight > 0f && validTypes.Contains(entry.type))
            {
                currentSum += entry.weight;
                if (randomPoint <= currentSum) return entry.type;
            }
        }

        return validTypes[0];
    }



    // Se le da un tipo, y devueve una rareza
    private ItemTier PickCachedTier(ItemType selectedType, Dictionary<ItemTier, List<GameObject>> tierDict)
    {
        float totalTierWeight = totalValidTierWeightByType[selectedType];

        if (totalTierWeight <= 0f)
        {
            var fallbackTiers = availableTiersByTypeCache[selectedType];
            return fallbackTiers[UnityEngine.Random.Range(0, fallbackTiers.Count)];
        }

        float randomPoint = UnityEngine.Random.Range(0f, totalTierWeight);
        float currentSum = 0f;

        for (int i = 0; i < tierProbabilities.Length; i++)
        {
            var entry = tierProbabilities[i];
            if (tierDict.ContainsKey(entry.tier) && entry.weight > 0f)
            {
                currentSum += entry.weight;
                if (randomPoint <= currentSum)
                {
                    return entry.tier;
                }
            }
        }

        return availableTiersByTypeCache[selectedType][0];
    }

    // Variacion del metodo superior utilizado al buscar objetos con filtro
    private ItemTier PickFilteredTier(List<ItemTier> validTiers)
    {
        if (validTiers.Count == 1) return validTiers[0];

        float totalWeight = 0f;
        for (int i = 0; i < tierProbabilities.Length; i++)
        {
            var entry = tierProbabilities[i];
            if (entry.weight > 0f && validTiers.Contains(entry.tier))
            {
                totalWeight += entry.weight;
            }
        }

        if (totalWeight <= 0f)
        {
            return validTiers[UnityEngine.Random.Range(0, validTiers.Count)];
        }

        float randomPoint = UnityEngine.Random.Range(0f, totalWeight);
        float currentSum = 0f;

        for (int i = 0; i < tierProbabilities.Length; i++)
        {
            var entry = tierProbabilities[i];
            if (entry.weight > 0f && validTiers.Contains(entry.tier))
            {
                currentSum += entry.weight;
                if (randomPoint <= currentSum) return entry.tier;
            }
        }

        return validTiers[0];
    }
    #endregion
}