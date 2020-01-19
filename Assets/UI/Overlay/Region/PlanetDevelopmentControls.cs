using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetDevelopmentControls : OverlayObject
{
    public Planet planet;
    public PlanetDevelopmentLabels planetDevelopmentLabels;
    public TileControl[] TileControls;
    // Yield Sprites
    public Sprite EnergyLowYieldSprite, EnergyMediumYieldSprite, EnergyHighYieldSprite, EnergyUncommonYieldSprite, EnergyRareYieldSprite,
                    WaterLowYieldSprite, WaterMediumYieldSprite, WaterHighYieldSprite, WaterUncommonYieldSprite, WaterRareYieldSprite,
                    FoodLowYieldSprite, FoodMediumYieldSprite, FoodHighYieldSprite, FoodUncommonYieldSprite, FoodRareYieldSprite,
                    MineralsLowYieldSprite, MineralsMediumYieldSprite, MineralsHighYieldSprite, MineralsUncommonYieldSprite, MineralsRareYieldSprite;
    public Button palaceButton;
    public PalacePanel palacePanel;
    // Dictionary for quick lookup of yield sprites
    private Dictionary<Resource, Dictionary<Yield, Sprite>> yieldSprites;
    // Dictionary containing the images for all 5 buttons for each tile
    private Dictionary<TileControl, Image[]> tileButtons;

    void OnEnable()
    {
        // This skips if the planet hasn't been selected
        if (planet == null)
        {
            return;
        }

        // The first time this runs we need to initialize some dictionaries
        if (tileButtons == null)
        {
            InitializeYieldTexts();
            GetTileButtons();
        }

        // Enable and update the tile controls we need
        for (int i = 0; i < planet.tiles.Length; i++)
        {
            // The planet tile in question
            Tile tile = planet.tiles[i];
            Resource[] tileResources = new Resource[tile.resources.Count];
            tile.resources.Keys.CopyTo(tileResources, 0);

            // Set the first yield icon
            tileButtons[TileControls[i]][0].sprite = yieldSprites[tileResources[0]][tile.resources[tileResources[0]]];

            if (tileResources.Length > 1)
            {
                // Set the second yield icon
                tileButtons[TileControls[i]][1].sprite = yieldSprites[tileResources[1]][tile.resources[tileResources[1]]];
                tileButtons[TileControls[i]][1].gameObject.SetActive(true);
            }
            else
            {
                tileButtons[TileControls[i]][1].gameObject.SetActive(false);
            }

            // Update the tile control's color and update the background mask to show development
            if (tile.development != null)
            {
                TileControls[i].TileOutline.color = ResourceUtil.ResourceColors[tile.development.resource];
                if (tile.development.tileDevelopments.ContainsKey(tile))
                {
                    float development = tile.development.tileDevelopments[tile];
                    TileControls[i].BackgroundMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, development / 100f * .15f);
                }
                else
                {
                    TileControls[i].BackgroundMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
                }
            }
            else
            {
                TileControls[i].TileOutline.color = Color.white;
                TileControls[i].BackgroundMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            }

            TileControls[i].gameObject.SetActive(true);
        }

        // Disable the tile controls we don't need
        if (planet.tiles.Length < TileControls.Length)
        for (int i = planet.tiles.Length; i < TileControls.Length; i++)
        {
            TileControls[i].gameObject.SetActive(false);
        }

        // Activate the palace button if needed
        if (planet.palace != null)
        {
            palaceButton.gameObject.SetActive(true);
        }
        else
        {
            palaceButton.gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        foreach(TileControl t in TileControls)
        {
            t.ClosePopups();
        }

        palacePanel.gameObject.SetActive(false);
    }

    public override void Initialize(MonoBehaviour viewObject)
    {
        if (viewObject is Region)
        {
            planet = (Planet)((Region)viewObject).orbitalObject;
            base.Initialize(viewObject);
        }
    }

    public void OnYieldButtonClick(TileControl tile, int yieldButton)
    {
        // The tile in question
        Tile planetTile = planet.tiles[Array.IndexOf(TileControls, tile)];
        Resource[] tileResources = new Resource[planetTile.resources.Count];
        planetTile.resources.Keys.CopyTo(tileResources, 0);
        // The resource that was clicked on
        Resource clickedResource = tileResources[yieldButton];
        AllocateTile(planetTile, clickedResource);

        // Update the tile control's color
        tile.TileOutline.color = ResourceUtil.ResourceColors[clickedResource];
                
        // Update the planet labels
        planetDevelopmentLabels.UpdateLabels();
    }

    public void OnCancelButtonClick(TileControl tile)
    {
        // The tile in question
        Tile planetTile = planet.tiles[Array.IndexOf(TileControls, tile)];

        if (planetTile.development != null)
        {
            planetTile.development.tiles.Remove(planetTile);
            planetTile.development = null;
                
            // Update the planet labels
            planetDevelopmentLabels.UpdateLabels();

            // Update the tile control's color
            tile.TileOutline.color = Color.white;
        }
    }

    public void OnCivilianIndustryButtonClick(TileControl tile)
    {
        // The tile in question
        Tile planetTile = planet.tiles[Array.IndexOf(TileControls, tile)];

        AllocateTile(planetTile, Resource.CivilianGoods);

        // Update the tile control's color
        tile.TileOutline.color = ResourceUtil.ResourceColors[Resource.CivilianGoods];
    }

    public void OnMilitaryIndustryButtonClick(TileControl tile)
    {
        // The tile in question
        Tile planetTile = planet.tiles[Array.IndexOf(TileControls, tile)];

        AllocateTile(planetTile, Resource.MilitaryGoods);

        // Update the tile control's color
        tile.TileOutline.color = ResourceUtil.ResourceColors[Resource.MilitaryGoods];
    }

    public void OnShipyardIndustryButtonClick(TileControl tile)
    {
        // The tile in question
        Tile planetTile = planet.tiles[Array.IndexOf(TileControls, tile)];

        AllocateTile(planetTile, Resource.ShipParts);

        // Update the tile control's color
        tile.TileOutline.color = ResourceUtil.ResourceColors[Resource.ShipParts];
    }

    public void OnTransportHubButtonClick(TileControl tile)
    {
        // The tile in question
        Tile planetTile = planet.tiles[Array.IndexOf(TileControls, tile)];

        AllocateTile(planetTile, Resource.TransportCapacity);

        // Update the tile control's color
        tile.TileOutline.color = ResourceUtil.ResourceColors[Resource.TransportCapacity];
    }

    public void OnPopupButtonClick(TileControl tile)
    {
        foreach(TileControl t in TileControls)
        {
            if (t != tile)
            {
                t.ClosePopups();
            }
        }

        palacePanel.gameObject.SetActive(false);
    }

    public void OnPalaceButtonClick()
    {
        // Close it if it's already open
        if (palacePanel.gameObject.activeSelf)
        {
            palacePanel.gameObject.SetActive(false);
        }
        else
        {
            palacePanel.palace = planet.palace;
            palacePanel.gameObject.SetActive(true);
        }
        // Close the other popups
        foreach(TileControl t in TileControls)
        {
            t.ClosePopups();
        }
    }



    private void InitializeYieldTexts()
    {
        yieldSprites = new Dictionary<Resource, Dictionary<Yield, Sprite>>()
        {
            {Resource.Energy, new Dictionary<Yield, Sprite>()
            {
                {Yield.Low, EnergyLowYieldSprite},
                {Yield.Medium, EnergyMediumYieldSprite},
                {Yield.High, EnergyHighYieldSprite},
                {Yield.Uncommon, EnergyUncommonYieldSprite},
                {Yield.Rare, EnergyRareYieldSprite}
            }},
            {Resource.Water, new Dictionary<Yield, Sprite>()
            {
                {Yield.Low, WaterLowYieldSprite},
                {Yield.Medium, WaterMediumYieldSprite},
                {Yield.High, WaterHighYieldSprite},
                {Yield.Uncommon, WaterUncommonYieldSprite},
                {Yield.Rare, WaterRareYieldSprite}
            }},
            {Resource.Food, new Dictionary<Yield, Sprite>()
            {
                {Yield.Low, FoodLowYieldSprite},
                {Yield.Medium, FoodMediumYieldSprite},
                {Yield.High, FoodHighYieldSprite},
                {Yield.Uncommon, FoodUncommonYieldSprite},
                {Yield.Rare, FoodRareYieldSprite}
            }},
            {Resource.Minerals, new Dictionary<Yield, Sprite>()
            {
                {Yield.Low, MineralsLowYieldSprite},
                {Yield.Medium, MineralsMediumYieldSprite},
                {Yield.High, MineralsHighYieldSprite},
                {Yield.Uncommon, MineralsUncommonYieldSprite},
                {Yield.Rare, MineralsRareYieldSprite}
            }}
        };
    }

    private void GetTileButtons()
    {
        tileButtons = new Dictionary<TileControl, Image[]>();

        foreach (TileControl t in TileControls)
        {
            Image[] buttons = t.buttons;
            tileButtons.Add(t, buttons);
        }
    }

    private void AllocateTile(Tile tile, Resource resource)
    {
        // The planet might not have this development yet
        if (!planet.developments.ContainsKey(resource))
        {
            // If it's a basic industry
            if ((int)resource < 100)
            {
                planet.developments.Add(resource, new BasicIndustry(resource));
            }
            // If it's a basic or advanced industry
            if ((int)resource < 200)
            {
                planet.developments.Add(resource, new AdvancedIndustry(resource));
            }
            if (resource == Resource.TransportCapacity)
            {
                planet.developments.Add(resource, new TransportHub(resource));
            }
        }

        // The development to allocate the tile to
        Development selectedDevelopment = planet.developments[resource];
        if (tile.development != null)
        {
            if (tile.development != selectedDevelopment)
            {
                tile.development.tiles.Remove(tile);
                selectedDevelopment.tiles.Add(tile);
                tile.development = selectedDevelopment;
            }
            // Else tile is already assigned to this development so do nothing
        }
        else
        {
            selectedDevelopment.tiles.Add(tile);
            tile.development = selectedDevelopment;
        }
    }
}
