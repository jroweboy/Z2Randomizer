﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Z2Randomizer
{
    class WestHyrule : World
    {
        public Location start;
        public Location fairy;
        public Location bagu;
        public Location jump;
        public Location medCave;
        public Location trophyCave;
        public Location palace1;
        public Location palace2;
        public Location palace3;
        public Location jar;
        public Location heart1;
        public Location heart2;
        public Location lifeNorth;
        public Location lifeSouth;
        public Location shieldTown;
        public Location bridge1;
        public Location bridge2;
        public Location pbagCave;
        private int bridgeCount;

        private Dictionary<Location, Location> bridgeConn;
        private Dictionary<Location, Location> cityConn;
        private Dictionary<Location, Location> caveConn;
        private Dictionary<Location, Location> graveConn;

        private List<Location> lostWoods;

        private readonly SortedDictionary<int, Terrain> terrains = new SortedDictionary<int, Terrain>
            {
                { 0x462F, Terrain.PALACE},
                { 0x4630, Terrain.CAVE },
                { 0x4631, Terrain.FOREST},
                { 0x4632, Terrain.CAVE },
                { 0x4633, Terrain.FOREST },
                { 0x4634, Terrain.GRASS },
                { 0x4635, Terrain.FOREST },
                { 0x4636, Terrain.ROAD },
                { 0x4637, Terrain.SWAMP },
                { 0x4638, Terrain.GRAVE },
                { 0x4639, Terrain.CAVE },
                { 0x463A, Terrain.CAVE },
                { 0x463B, Terrain.CAVE },
                { 0x463C, Terrain.CAVE },
                { 0x463D, Terrain.CAVE },
                { 0x463E, Terrain.CAVE },
                { 0x463F, Terrain.CAVE },
                { 0x4640, Terrain.GRAVE },
                { 0x4641, Terrain.CAVE },
                { 0x4642, Terrain.BRIDGE },
                { 0x4643, Terrain.BRIDGE },
                { 0x4644, Terrain.BRIDGE },
                { 0x4645, Terrain.BRIDGE },
                { 0x4646, Terrain.FOREST },
                { 0x4647, Terrain.SWAMP },
                { 0x4648, Terrain.FOREST },
                { 0x4649, Terrain.FOREST },
                { 0x464A, Terrain.FOREST },
                { 0x464B, Terrain.FOREST },
                { 0x464C, Terrain.FOREST },
                { 0x464D, Terrain.ROAD },
                //{ 0x464E, terrain.desert },
                { 0x464F, Terrain.DESERT },
                //{ 0x4658, terrain.bridge },
                //{ 0x4659, terrain.cave },
                //{ 0x465A, terrain.cave },
                { 0x465B, Terrain.GRAVE },
                { 0x465C, Terrain.TOWN },
                { 0x465E, Terrain.TOWN },
                { 0x465F, Terrain.TOWN },
                { 0x4660, Terrain.TOWN },
                { 0x4661, Terrain.FOREST },
                { 0x4662, Terrain.TOWN },
                { 0x4663, Terrain.PALACE },
                { 0x4664, Terrain.PALACE },
                { 0x4665, Terrain.PALACE }
        };

        private const int MAP_ADDR = 0x7480;

        public WestHyrule(Hyrule hy)
            : base(hy)
        {
            LoadLocations(0x4639, 4, terrains, Continent.WEST);
            LoadLocations(0x4640, 2, terrains, Continent.WEST);

            LoadLocations(0x462F, 10, terrains, Continent.WEST);
            LoadLocations(0x463D, 3, terrains, Continent.WEST);
            LoadLocations(0x4642, 12, terrains, Continent.WEST);
            LoadLocations(0x464F, 1, terrains, Continent.WEST);
            LoadLocations(0x465B, 2, terrains, Continent.WEST);
            LoadLocations(0x465E, 8, terrains, Continent.WEST);
            start = GetLocationByMap(0x80, 0x00);
            reachableAreas = new HashSet<string>();
            Location jumpCave = GetLocationByMap(9, 0);
            jumpCave.NeedJump = true;
            medCave = GetLocationByMap(0x0E, 0);
            Location heartCave = GetLocationByMap(0x10, 0);
            Location fairyCave = GetLocationByMap(0x12, 0);
            fairyCave.NeedFairy = true;
            jump = GetLocationByMap(0xC5, 4);
            bagu = GetLocationByMap(0x18, 4);
            fairy = GetLocationByMap(0xCB, 4);
            lifeNorth = GetLocationByMap(0xC8, 4);
            lifeSouth = GetLocationByMap(0x06, 4);
            lifeNorth.NeedBagu = true;
            lifeSouth.NeedBagu = true;
            trophyCave = GetLocationByMap(0xE1, 0);
            raft = GetLocationByMem(0x4658);
            palace1 = GetLocationByMem(0x4663);
            palace1.PalNum = 1;
            palace2 = GetLocationByMem(0x4664);
            palace2.PalNum = 2;
            palace3 = GetLocationByMem(0x4665);
            palace3.PalNum = 3;
            jar = GetLocationByMem(0x4632);
            heart1 = GetLocationByMem(0x463F);
            heart2 = GetLocationByMem(0x4634);
            shieldTown = GetLocationByMem(0x465C);
            pbagCave = GetLocationByMem(0x463D);


            Location parapaCave1 = GetLocationByMap(07, 0);
            Location parapaCave2 = GetLocationByMap(0xC7, 0);
            Location jumpCave2 = GetLocationByMap(0xCB, 0);
            Location fairyCave2 = GetLocationByMap(0xD3, 0);
            bridge1 = GetLocationByMap(0x04, 0);
            bridge2 = GetLocationByMap(0xC5, 0);

            if (hy.Props.saneCaves)
            {
                fairyCave.TerrainType = Terrain.CAVE;
            }

            caveConn = new Dictionary<Location, Location>();
            bridgeConn = new Dictionary<Location, Location>();
            cityConn = new Dictionary<Location, Location>();
            graveConn = new Dictionary<Location, Location>();

            //connections.Add(hammerEnter, hammerExit);
            //connections.Add(hammerExit, hammerEnter);
            //caveConn.Add(hammerEnter, hammerExit);
            //caveConn.Add(hammerExit, hammerEnter);
            connections.Add(parapaCave1, parapaCave2);
            connections.Add(parapaCave2, parapaCave1);
            caveConn.Add(parapaCave1, parapaCave2);
            caveConn.Add(parapaCave2, parapaCave1);
            connections.Add(jumpCave, jumpCave2);
            connections.Add(jumpCave2, jumpCave);
            caveConn.Add(jumpCave, jumpCave2);
            caveConn.Add(jumpCave2, jumpCave);
            connections.Add(fairyCave, fairyCave2);
            connections.Add(fairyCave2, fairyCave);
            caveConn.Add(fairyCave2, fairyCave);
            graveConn.Add(fairyCave, fairyCave2);
            connections.Add(lifeNorth, lifeSouth);
            connections.Add(lifeSouth, lifeNorth);
            cityConn.Add(lifeSouth, lifeNorth);
            cityConn.Add(lifeNorth, lifeSouth);
            connections.Add(bridge1, bridge2);
            connections.Add(bridge2, bridge1);
            bridgeConn.Add(bridge1, bridge2);
            bridgeConn.Add(bridge2, bridge1);

            enemies = new List<int> { 3, 4, 5, 17, 18, 20, 21, 22, 23, 24, 25, 26, 27, 28, 31, 32 };
            flyingEnemies = new List<int> { 0x06, 0x07, 0x0A, 0x0D, 0x0E };
            generators = new List<int> { 11, 12, 15, 29 };
            shorties = new List<int> { 3, 4, 5, 17, 18, 0x1C, 0x1F };
            tallGuys = new List<int> { 0x20, 20, 21, 22, 23, 24, 25, 26, 27 };
            enemyAddr = 0x48B0;
            enemyPtr = 0x45B1;

            overworldMaps = new List<int>() { 0x22, 0x1D, 0x27, 0x30, 0x23, 0x3A, 0x1E, 0x35, 0x28 };
            MAP_ROWS = 75;
            MAP_COLS = 64;
            baseAddr = 0x462F;
            VANILLA_MAP_ADDR = 0x506C;

            walkable = new List<Terrain>() { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE };
            randomTerrains = new List<Terrain> { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE, Terrain.MOUNAIN };
            if (hy.Props.hideLocs)
            {
                unimportantLocs.Add(GetLocationByMem(0x4631));
                unimportantLocs.Add(GetLocationByMem(0x4633));
                unimportantLocs.Add(GetLocationByMem(0x4635));
                unimportantLocs.Add(GetLocationByMem(0x4637));
                unimportantLocs.Add(GetLocationByMem(0x4638));
                unimportantLocs.Add(GetLocationByMem(0x4646));
                unimportantLocs.Add(GetLocationByMem(0x4647));
                unimportantLocs.Add(GetLocationByMem(0x4648));
                unimportantLocs.Add(GetLocationByMem(0x4649));
                unimportantLocs.Add(GetLocationByMem(0x464A));
                unimportantLocs.Add(GetLocationByMem(0x464B));
                unimportantLocs.Add(GetLocationByMem(0x464C));
                unimportantLocs.Add(GetLocationByMem(0x464D));
                unimportantLocs.Add(GetLocationByMem(0x464F));
                if(!hy.Props.helpfulHints)
                {
                    unimportantLocs.Add(GetLocationByMem(0x465B));
                }
            }
            if (hy.Props.westBiome.Equals("Islands"))
            {
                this.biome = Biome.islands;
            }
            else if (hy.Props.westBiome.Equals("Canyon") || hy.Props.westBiome.Equals("CanyonD"))
            {
                this.biome = Biome.canyon;
            }
            else if (hy.Props.westBiome.Equals("Mountainous"))
            {
                this.biome = Biome.mountainous;
            }
            else if(hy.Props.westBiome.Equals("Caldera"))
            {
                this.biome = Biome.caldera;
            }
            else if(hy.Props.westBiome.Equals("Mountainous"))
            {
                this.biome = Biome.mountainous;
            }
            else if (hy.Props.westBiome.Equals("Vanilla"))
            {
                this.biome = Biome.vanilla;
            }
            else if (hy.Props.westBiome.Equals("Vanilla (shuffled)"))
            {
                this.biome = Biome.vanillaShuffle;
            }
            else
            {
                this.biome = Biome.vanillalike;
            }

            section = new SortedDictionary<Tuple<int, int>, string>{
                { Tuple.Create(0x34, 0x17), "north" },
            { Tuple.Create(0x20, 0x1D), "north" },
            { Tuple.Create(0x2A, 0x25), "north" },
            { Tuple.Create(0x3C, 0x10), "north" },
            { Tuple.Create(0x56, 0x14), "mid" },
            { Tuple.Create(0x40, 0x3E), "parapa" },
            { Tuple.Create(0x4D, 0x15), "mid" },
            { Tuple.Create(0x39, 0x3D), "parapa" },
            { Tuple.Create(0x47, 0x08), "mid" },
            { Tuple.Create(0x5C, 0x30), "grave" },
            { Tuple.Create(0x29, 0x30), "parapa" },
            { Tuple.Create(0x2E, 0x37), "north" },
            { Tuple.Create(0x3A, 0x01), "north" },
            { Tuple.Create(0x3E, 0x03), "mid" },
            { Tuple.Create(0x3E, 0x26), "mid" },
            { Tuple.Create(0x45, 0x09), "hammer0" },
            { Tuple.Create(0x3E, 0x36), "hammer" },
            { Tuple.Create(0x60, 0x32), "grave" },
            { Tuple.Create(0x66, 0x3B), "island" },
            { Tuple.Create(0x52, 0x10), "mid" },
            { Tuple.Create(0x57, 0x1A), "mid" },
            { Tuple.Create(0x61, 0x1A), "dmexit" },
            { Tuple.Create(0x61, 0x22), "grave" },
            { Tuple.Create(0x40, 0x07), "mid" },
            { Tuple.Create(0x43, 0x11), "mid" },
            { Tuple.Create(0x57, 0x21), "mid" },
            { Tuple.Create(0x4C, 0x14), "mid" },
            { Tuple.Create(0x4D, 0x11), "mid" },
            { Tuple.Create(0x4E, 0x13), "mid" },
            { Tuple.Create(0x4D, 0x17), "mid" },
            { Tuple.Create(0x44, 0x25), "mid" },
            { Tuple.Create(0x66, 0x26), "grave" },
            { Tuple.Create(0x4D, 0x3D), "grave" },
            { Tuple.Create(0x5F, 0x0A), "lifesouth" },
            { Tuple.Create(0x60, 0x15), "dmexit" },
            { Tuple.Create(0x58, 0x32), "grave" },
            { Tuple.Create(0x36, 0x2E), "north" },
            { Tuple.Create(0x24, 0x02), "north" },
            { Tuple.Create(0x5B, 0x08), "lifesouth" },
            { Tuple.Create(0x59, 0x08), "mid" },
            { Tuple.Create(0x4C, 0x15), "mid" },
            { Tuple.Create(0x4B, 0x3C), "grave" },
            { Tuple.Create(0x20, 0x3E), "parapa" },
            { Tuple.Create(0x40, 0x0B), "mid" },
            { Tuple.Create(0x62, 0x39), "island" }
            };
            lostWoods = new List<Location> { GetLocationByMem(0x4649), GetLocationByMem(0x464A), GetLocationByMem(0x464B), GetLocationByMem(0x464C), GetLocationByMem(0x4635) };
        }

        public bool terraform()
        {
            foreach (Location location in AllLocations)
            {
                location.CanShuffle = true;
            }
            if (this.biome == Biome.vanilla || this.biome == Biome.vanillaShuffle)
            {
                MAP_ROWS = 75;
                MAP_COLS = 64;
                ReadVanillaMap();
                if(this.biome == Biome.vanillaShuffle)
                {
                    areasByLocation = new SortedDictionary<string, List<Location>>();

                    areasByLocation.Add("north", new List<Location>());
                    areasByLocation.Add("mid", new List<Location>());
                    areasByLocation.Add("parapa", new List<Location>());
                    areasByLocation.Add("grave", new List<Location>());
                    areasByLocation.Add("lifesouth", new List<Location>());
                    areasByLocation.Add("island", new List<Location>());
                    areasByLocation.Add("hammer", new List<Location>());
                    areasByLocation.Add("hammer0", new List<Location>());
                    areasByLocation.Add("dmexit", new List<Location>());
                    foreach (Location location in AllLocations)
                    {
                        areasByLocation[section[location.Coords]].Add(GetLocationByCoords(location.Coords));
                    }
                    ChooseConn("parapa", connections, true);
                    ChooseConn("lifesouth", connections, true);
                    ChooseConn("island", connections, true);
                    ChooseConn("dmexit", connections, true);

                    ShuffleLocations(AllLocations);
                    if (hy.Props.vanillaOriginal)
                    {
                        foreach (Location location in AllLocations)
                        {
                            map[location.Ypos - 30, location.Xpos] = location.TerrainType;
                        }
                    }
                    foreach(Location location in Locations[Terrain.CAVE])
                    {
                        location.PassThrough = 0;
                    }
                    foreach (Location location in Locations[Terrain.TOWN])
                    {
                        location.PassThrough = 0;
                    }
                    foreach (Location location in Locations[Terrain.PALACE])
                    {
                        location.PassThrough = 0;
                    }
                    raft.PassThrough = 0;
                    bridge1.PassThrough = 0;
                    bridge2.PassThrough = 0;
                    GetLocationByMap(0x12, 0).PassThrough = 0; //fairy cave

                }
            }
            else
            {
                Terrain water = Terrain.WATER;
                if(hy.Props.bootsWater)
                {
                    water = Terrain.WALKABLEWATER;
                }

                bcount = 2000;

                if(hy.Props.bagusWoods)
                {
                    bagu.CanShuffle = false;
                    foreach(Location location in lostWoods)
                    {
                        location.CanShuffle = false;
                        unimportantLocs.Remove(location);
                    }
                }
                while (bcount > MAP_SIZE_BYTES)
                {
                    Terrain riverT = Terrain.MOUNAIN;
                    lifeSouth.CanShuffle = false;
                    lifeNorth.CanShuffle = false;

                    map = new Terrain[MAP_ROWS, MAP_COLS];

                    for (int i = 0; i < MAP_ROWS; i++)
                    {
                        for (int j = 0; j < MAP_COLS; j++)
                        {
                            map[i, j] = Terrain.NONE;
                        }
                    }

                    if (this.biome == Biome.islands)
                    {
                        riverT = water;
                        for (int i = 0; i < MAP_COLS; i++)
                        {
                            map[0, i] = water;
                            map[MAP_ROWS - 1, i] = water;
                        }

                        for (int i = 0; i < MAP_ROWS; i++)
                        {
                            map[i, 0] = water;
                            map[i, MAP_COLS - 1] = water;
                        }


                        int cols = hy.RNG.Next(2, 4);
                        int rows = hy.RNG.Next(2, 4);
                        List<int> pickedC = new List<int>();
                        List<int> pickedR = new List<int>();

                        while (cols > 0)
                        {
                            int col = hy.RNG.Next(10, MAP_COLS - 11);
                            if (!pickedC.Contains(col))
                            {
                                for (int i = 0; i < MAP_ROWS; i++)
                                {
                                    if (map[i, col] == Terrain.NONE)
                                    {
                                        map[i, col] = water;
                                    }
                                }
                                pickedC.Add(col);
                                cols--;
                            }
                        }

                        while (rows > 0)
                        {
                            int row = hy.RNG.Next(10, MAP_ROWS - 11);
                            if (!pickedR.Contains(row))
                            {
                                for (int i = 0; i < MAP_COLS; i++)
                                {
                                    if (map[row, i] == Terrain.NONE)
                                    {
                                        map[row, i] = water;
                                    }
                                }
                                pickedR.Add(row);
                                rows--;
                            }
                        }
                        lifeSouth.CanShuffle = false;
                        lifeNorth.CanShuffle = false;
                        walkable = new List<Terrain>() { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE };
                        randomTerrains = new List<Terrain> { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE, Terrain.MOUNAIN, water };

                    }
                    else if (this.biome == Biome.canyon)
                    {
                        horizontal = hy.RNG.NextDouble() > .5;
                        riverT = water;
                        if (hy.Props.westBiome.Equals("CanyonD"))
                        {
                            riverT = Terrain.DESERT;
                            bridge1.CanShuffle = false;
                            bridge1.Ypos = 0;
                            bridge2.CanShuffle = false;
                            bridge2.Ypos = 0;
                        }
                        walkable = new List<Terrain>() { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST,  Terrain.GRAVE, Terrain.MOUNAIN };
                        randomTerrains = new List<Terrain> { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST,  Terrain.GRAVE, Terrain.MOUNAIN, water };


                        DrawCanyon(riverT);
                        this.walkable.Remove(Terrain.MOUNAIN);

                        //this.randomTerrains.Add(terrain.lava);

                    }
                    else if (this.biome == Biome.caldera)
                    {
                        this.horizontal = hy.RNG.NextDouble() > .5;
                        DrawCenterMountain();
                        palace3.CanShuffle = false;
                        walkable = new List<Terrain>() { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE };
                        randomTerrains = new List<Terrain> { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE, Terrain.MOUNAIN, water };
                    }
                    else if (this.biome == Biome.mountainous)
                    {
                        walkable = new List<Terrain>() { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE };
                        randomTerrains = new List<Terrain> { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE, Terrain.MOUNAIN, water };

                        riverT = Terrain.MOUNAIN;
                        for (int i = 0; i < MAP_COLS; i++)
                        {
                            map[0, i] = Terrain.MOUNAIN;
                            map[MAP_ROWS - 1, i] = Terrain.MOUNAIN;
                        }

                        for (int i = 0; i < MAP_ROWS; i++)
                        {
                            map[i, 0] = Terrain.MOUNAIN;
                            map[i, MAP_COLS - 1] = Terrain.MOUNAIN;
                        }


                        int cols = hy.RNG.Next(2, 4);
                        int rows = hy.RNG.Next(2, 4);
                        List<int> pickedC = new List<int>();
                        List<int> pickedR = new List<int>();

                        while (cols > 0)
                        {
                            int col = hy.RNG.Next(10, MAP_COLS - 11);
                            if (!pickedC.Contains(col))
                            {
                                for (int i = 0; i < MAP_ROWS; i++)
                                {
                                    if (map[i, col] == Terrain.NONE)
                                    {
                                        map[i, col] = Terrain.MOUNAIN;
                                    }
                                }
                                pickedC.Add(col);
                                cols--;
                            }
                        }

                        while (rows > 0)
                        {
                            int row = hy.RNG.Next(10, MAP_ROWS - 11);
                            if (!pickedR.Contains(row))
                            {
                                for (int i = 0; i < MAP_COLS; i++)
                                {
                                    if (map[row, i] == Terrain.NONE)
                                    {
                                        map[row, i] = Terrain.MOUNAIN;
                                    }
                                }
                                pickedR.Add(row);
                                rows--;
                            }
                        }
                        lifeSouth.CanShuffle = false;
                        lifeNorth.CanShuffle = false;
                        
                    }
                    else
                    {
                        walkable = new List<Terrain>() { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE };
                        randomTerrains = new List<Terrain> { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE, Terrain.MOUNAIN, water };
                        //drawRoad();
                        drawMountains();
                        //drawBridge();
                        DrawRiver(new List<Location>() { GetLocationByMem(0x4642), GetLocationByMem(0x4643) });
                    }

                    Direction rDir = Direction.east;
                    if (!hy.Props.continentConnections.Equals("Normal") && this.biome != Biome.canyon)
                    {
                        rDir = (Direction)hy.RNG.Next(4);
                    }
                    else if (this.biome == Biome.canyon)
                    {
                        rDir = (Direction)hy.RNG.Next(2);
                        if (horizontal)
                        {
                            rDir += 2;
                        }
                    }
                    if (raft != null)
                    {
                        DrawOcean(rDir);
                    }


                    Direction bDir = (Direction)hy.RNG.Next(4);
                    do
                    {
                        if (this.biome != Biome.canyon && this.biome != Biome.caldera)
                        {
                            bDir = (Direction)hy.RNG.Next(4);
                        }
                        else
                        {
                            bDir = (Direction)hy.RNG.Next(2);
                            if (horizontal)
                            {
                                bDir += 2;
                            }
                        }
                    } while (bDir == rDir);
                    if (bridge != null)
                    {
                        DrawOcean(bDir);
                    }
                    Boolean b = PlaceLocations(riverT);
                    if (!b)
                    {
                        return false;
                    }

                    if (hy.Props.bagusWoods)
                    {
                        bool f = placeBagu();
                        if (!f)
                        {
                            return false;
                        }
                    }

                    if (hy.Props.hideLocs)
                    {
                        PlaceRandomTerrain(50);
                    }
                    else
                    {
                        PlaceRandomTerrain(15);
                    }


                    if (!GrowTerrain())
                    {
                        return false;
                    }


                    if (raft != null)
                    {
                        Boolean r = DrawRaft(false, rDir);
                        if (!r)
                        {
                            return false;
                        }
                    }

                    if (bridge != null)
                    {
                        Boolean b2 = DrawRaft(true, bDir);
                        if (!b2)
                        {
                            return false;
                        }
                    }
                    
                    if (this.biome == Biome.caldera)
                    {

                        bool f = ConnectIslands(1, true, water, false, false, false);
                        if (!f)
                        {
                            return false;
                        }

                        bool g = makeCaldera();
                        if (!g)
                        {
                            return false;
                        }
                    }
                    PlaceRocks();
                    
                    placeHiddenLocations();

                    int bridges = 10;

                    if (this.biome == Biome.canyon)
                    {
                        bridges = 100;
                        bool f = ConnectIslands(bridges, true, riverT, false, true, false);
                        if (!f)
                        {
                            return false;
                        }
                    }
                    if (this.biome == Biome.islands)
                    {
                        bridges = 25;
                        bool f = ConnectIslands(bridges, true, riverT, false, true, false);
                        if (!f)
                        {
                            return false;
                        }
                    }
                    if (this.biome == Biome.mountainous)
                    {
                        bridges = 15;
                        this.walkable.Add(Terrain.ROAD);

                        bool h = ConnectIslands(bridges, true, riverT, false, false, false);
                        if (!h)
                        {
                            return false;
                        }
                    }
                    if (this.biome == Biome.vanillalike)
                    {
                        bridges = 4;
                        riverT = water;
                        ConnectIslands(2, false, Terrain.MOUNAIN, false, false, false);
                        bool f = ConnectIslands(bridges, true, riverT, false, true, false);
                        if (!f)
                        {
                            return false;
                        }
                    }




                    foreach (Location location in Locations[Terrain.ROAD])
                    {
                        if (location.CanShuffle)
                        {
                            location.Ypos = 0;
                            location.CanShuffle = false;
                        }
                    }

                    foreach (Location location in Locations[Terrain.BRIDGE])
                    {
                        if (location.CanShuffle)
                        {
                            location.Ypos = 0;
                            location.CanShuffle = false;
                        }
                    }


                    //check bytes and adjust
                    WriteBytes(false, MAP_ADDR, MAP_SIZE_BYTES, 0, 0);
                    Console.WriteLine("West:" + bcount);
                }
            }
            WriteBytes(true, MAP_ADDR, MAP_SIZE_BYTES, 0, 0);

            v = new bool[MAP_ROWS, MAP_COLS];
            for (int i = 0; i < MAP_ROWS; i++)
            {
                for (int j = 0; j < MAP_COLS; j++)
                {
                    v[i, j] = false;
                }
            }

            v[start.Ypos - 30, start.Xpos] = true;
            return true;
        }

        public void setStart()
        {
            v[start.Ypos - 30, start.Xpos] = true;
            start.Reachable = true;
        }

        private bool placeBagu()
        {
            int y = hy.RNG.Next(6, MAP_ROWS - 7);
            int x = hy.RNG.Next(6, MAP_COLS - 7);
            int tries = 0;
            while((map[y, x] != Terrain.NONE || GetLocationByCoords(Tuple.Create(y + 30, x)) != null) && tries < 1000)
            {
                y = hy.RNG.Next(6, MAP_ROWS - 7);
                x = hy.RNG.Next(6, MAP_COLS - 7);
            }
            if(tries >= 1000)
            {
                return false;
            }
            bagu.Ypos = y + 30;
            bagu.Xpos = x;
            map[y, x] = Terrain.FOREST;

            int placed = 0;
            tries = 0;
            while(placed < 5 && tries < 3000)
            {
                int newx = hy.RNG.Next(x - 3, x + 4);
                int newy = hy.RNG.Next(y - 3, y + 4);
                while((map[newy, newx] != Terrain.NONE || GetLocationByCoords(Tuple.Create(newy + 30, newx)) != null) && tries < 100)
                {
                    newx = hy.RNG.Next(x - 3, x + 4);
                    newy = hy.RNG.Next(y - 3, y + 4);
                    tries++;
                }
                lostWoods[placed].Ypos = newy + 30;
                lostWoods[placed].Xpos = newx;
                map[newy, newx] = Terrain.FOREST;
                placed++;
            }
            if(tries >= 3000 && placed < 3)
            {
                return false;
            }
            else
            {
                for(int i = placed; i < lostWoods.Count; i++)
                {
                    lostWoods[placed].Ypos = 0;
                }
            }
            return true;
        }
        private bool makeCaldera()
        {
            Terrain water = Terrain.WATER;
            if(hy.Props.bootsWater)
            {
                water = Terrain.WALKABLEWATER;
            }
            int centerx = hy.RNG.Next(21, 41);
            int centery = hy.RNG.Next(32, 42);
            if (horizontal)
            {
                centerx = hy.RNG.Next(27, 37);
                centery = hy.RNG.Next(22, 52);
            }

            bool placeable = false;
            do
            {
                if (horizontal)
                {
                    centerx = hy.RNG.Next(27, 37);
                    centery = hy.RNG.Next(22, 52);
                }
                else
                {
                    centerx = hy.RNG.Next(21, 41);
                    centery = hy.RNG.Next(32, 42);
                }
                placeable = true;
                for (int i = centery - 7; i < centery + 8; i++)
                {
                    for (int j = centerx - 7; j < centerx + 8; j++)
                    {
                        if (map[i, j] != Terrain.MOUNAIN)
                        {
                            placeable = false;
                        }
                    }
                }
            } while (!placeable);

            int startx = centerx - 5;
            int starty = centery;
            int deltax = 1;
            int deltay = 0;
            if (!horizontal)
            {
                startx = centerx;
                starty = centery - 5;
                deltax = 0;
                deltay = 1;
            }
            for(int i = 0; i < 10; i++)
            {
                int lake = hy.RNG.Next(7, 11);
                if(i == 0 || i == 9)
                {
                    lake = hy.RNG.Next(3, 6);
                }
                if (horizontal)
                {
                    for(int j = 0; j < lake / 2; j++)
                    {
                        map[starty + j, startx] = water;
                        if(i == 0)
                        {
                            map[starty + j, startx - 1] = Terrain.FOREST;
                        }
                        if(i == 9)
                        {
                            map[starty + j, startx + 1] = Terrain.FOREST;
                        }
                        
                    }
                    int top = starty + lake / 2;
                    while(map[top, startx - 1] == Terrain.MOUNAIN)
                    {
                        map[top, startx - 1] = Terrain.FOREST;
                        top--;
                    }
                    top = starty + lake / 2;
                    while (map[top, startx - 1] != Terrain.MOUNAIN)
                    {
                        map[top, startx] = Terrain.FOREST;
                        top++;
                    }

                    for (int j = 0; j < lake - (lake / 2); j++)
                    {
                        map[starty - j, startx] = water;
                        if (i == 0)
                        {
                            map[starty - j, startx - 1] = Terrain.FOREST;
                        }
                        if (i == 9)
                        {
                            map[starty - j, startx + 1] = Terrain.FOREST;
                        }
                        
                    }
                    top = starty - (lake - (lake / 2));
                    while (map[top, startx - 1] == Terrain.MOUNAIN)
                    {
                        map[top, startx - 1] = Terrain.FOREST;
                        top++;
                    }
                    top = starty - (lake - (lake / 2));
                    while (map[top, startx - 1] != Terrain.MOUNAIN)
                    {
                        map[top, startx] = Terrain.FOREST;
                        top--;
                    }

                    //map[starty + lake / 2, startx] = terrain.forest;
                   // map[starty - (lake - (lake / 2)), startx] = terrain.forest;
                    if (i == 0)
                    {
                        map[starty + lake / 2, startx + 1] = Terrain.FOREST;
                        map[starty - (lake - (lake / 2)), startx - 1] = Terrain.FOREST;
                    }
                    if (i == 9)
                    {
                        map[starty + lake / 2, startx + 1] = Terrain.FOREST;
                        map[starty - (lake - (lake / 2) ), startx + 1] = Terrain.FOREST;
                    }

                }
                else
                {
                    for (int j = 0; j < lake / 2; j++)
                    {
                        map[starty, startx + j] = water;
                        if (i == 0)
                        {
                            map[starty - 1, startx + j] = Terrain.FOREST;
                        }
                        if (i == 9)
                        {
                            map[starty + 1, startx + j] = Terrain.FOREST;
                        }
                    }
                    int top = startx + lake / 2;
                    while (map[starty - 1, top] == Terrain.MOUNAIN && i != 0)
                    {
                        map[starty - 1, top] = Terrain.FOREST;
                        top--;
                    }
                    top = startx + lake / 2;
                    while (map[starty - 1, top] != Terrain.MOUNAIN && i != 0)
                    {
                        map[starty, top] = Terrain.FOREST;
                        top++;
                    }

                    for (int j = 0; j < lake - (lake / 2); j++)
                    {
                        map[starty, startx - j] = water;
                        if (i == 0)
                        {
                            map[starty - 1, startx - j] = Terrain.FOREST;
                        }
                        if (i == 9)
                        {
                            map[starty + 1, startx - j] = Terrain.FOREST;
                        }
                    }
                     top = startx - (lake - (lake / 2));
                    while (map[starty - 1, top] == Terrain.MOUNAIN && i != 0)
                    {
                        map[starty - 1, top] = Terrain.FOREST;
                        top++;
                    }
                    top = startx - (lake - (lake / 2));
                    while (map[starty - 1, top] != Terrain.MOUNAIN && i != 0)
                    {
                        map[starty, top] = Terrain.FOREST;
                        top--;
                    }
                    //map[starty, startx + lake / 2] = terrain.forest;
                    //map[starty, startx - (lake - (lake / 2))] = terrain.forest;
                    if (i == 0)
                    {
                        map[starty - 1, startx + lake / 2] = Terrain.FOREST;
                        map[starty - 1, startx - (lake - (lake / 2))] = Terrain.FOREST;
                    }
                    if (i == 9)
                    {
                        map[starty + 1, startx + lake / 2] = Terrain.FOREST;
                        map[starty + 1, startx - (lake - (lake / 2))] = Terrain.FOREST;
                    }
                }
                startx += deltax;
                starty += deltay;
            }
            int caves = hy.RNG.Next(2) + 1;
            Location cave1l = new Location();
            Location cave1r = new Location();
            Location cave2l = new Location();
            Location cave2r = new Location();
            int numCaves = 2;
            if(hy.Props.saneCaves)
            {
                numCaves++;
            }
            int cavenum1 = hy.RNG.Next(numCaves);
            if(cavenum1 == 0)
            {
                cave1l = GetLocationByMap(9, 0);//jump cave
                cave1r = GetLocationByMap(0xCB, 0);
            }
            else if (cavenum1 == 1)
            {
                cave1l = GetLocationByMap(07, 0); //parappa
                cave1r = GetLocationByMap(0xC7, 0);
            }
            else
            {
                cave1l = GetLocationByMap(0x12, 0); //fairy cave
                cave1r = GetLocationByMap(0xD3, 0);
            }
            map[cave1l.Ypos - 30, cave1l.Xpos] = Terrain.MOUNAIN;
            map[cave1r.Ypos - 30, cave1r.Xpos] = Terrain.MOUNAIN;
            if (caves > 1)
            {
                int cavenum2 = hy.RNG.Next(numCaves);
                while(cavenum2 == cavenum1)
                {
                    cavenum2 = hy.RNG.Next(numCaves);
                }
                if (cavenum2 == 0)
                {
                    cave2l = GetLocationByMap(9, 0);//jump cave
                    cave2r = GetLocationByMap(0xCB, 0);
                }
                else if (cavenum2 == 1)
                {
                    cave2l = GetLocationByMap(07, 0); //parappa
                    cave2r = GetLocationByMap(0xC7, 0);
                }
                else
                {
                    cave2l = GetLocationByMap(0x12, 0); //fairy cave
                    cave2r = GetLocationByMap(0xD3, 0);
                }
                map[cave2l.Ypos - 30, cave2l.Xpos] = Terrain.MOUNAIN;
                map[cave2r.Ypos - 30, cave2r.Xpos] = Terrain.MOUNAIN;
            }
            int caveDir = hy.RNG.Next(2);
            if (horizontal)
            {
                bool f = HorizontalCave(caveDir, centerx, centery, cave1l, cave1r);
                if(!f)
                {
                    return false;
                }

                if(caves > 1)
                {
                    if(caveDir == 0)
                    {
                        caveDir = 1;
                    }
                    else
                    {
                        caveDir = 0;
                    }
                    f = HorizontalCave(caveDir, centerx, centery, cave2l, cave2r);
                    if (!f)
                    {
                        return false;
                    }
                }
                
                if(caves == 1)
                {
                    int delta = -1;
                    if(caveDir == 0) //palace goes right
                    {
                        delta = 1;
                    }
                    int palacex = centerx;
                    int palacey = hy.RNG.Next(centery - 2, centery + 3);
                    while (map[palacey, palacex] != Terrain.MOUNAIN)
                    {
                        palacex += delta;
                    }
                    map[palacey, palacex] = Terrain.PALACE;
                    palace3.Ypos = palacey + 30;
                    palace3.Xpos = palacex;
                    map[palacey, palacex + delta] = Terrain.MOUNAIN;

                }
                else
                {
                    int palaceDir = hy.RNG.Next(2);
                    int delta = -1;
                    if(palaceDir == 0)
                    {
                        delta = 1;
                    }
                    int palacex = hy.RNG.Next(centerx - 2, centerx + 3);
                    int palacey = centery;
                    while (map[palacey, palacex] != Terrain.MOUNAIN)
                    {
                        palacey += delta;
                    }
                    map[palacey, palacex] = Terrain.PALACE;
                    palace3.Ypos = palacey + 30;
                    palace3.Xpos = palacex;
                    map[palacey + delta, palacex] = Terrain.MOUNAIN;

                }

            }
            else
            {
                bool f = VerticalCave(caveDir, centerx, centery, cave1l, cave1r);
                if (!f)
                {
                    return false;
                }

                if (caves > 1)
                {
                    if (caveDir == 0)
                    {
                        caveDir = 1;
                    }
                    else
                    {
                        caveDir = 0;
                    }
                    f = VerticalCave(caveDir, centerx, centery, cave2l, cave2r);
                    if (!f)
                    {
                        return false;
                    }
                }

                if (caves == 1)
                {
                    int delta = -1;
                    if (caveDir == 0) //palace goes down
                    {
                        delta = 1;
                    }
                    int palacex = hy.RNG.Next(centerx - 2, centerx + 3);
                    int palacey = centery;
                    while (map[palacey, palacex] != Terrain.MOUNAIN)
                    {
                        palacey += delta;
                    }
                    map[palacey, palacex] = Terrain.PALACE;
                    palace3.Ypos = palacey + 30;
                    palace3.Xpos = palacex;
                    map[palacey + delta, palacex] = Terrain.MOUNAIN;


                }
                else
                {
                    int palaceDir = hy.RNG.Next(2);
                    int delta = -1;
                    if (palaceDir == 0)
                    {
                        delta = 1;
                    }
                    int palacex = centerx;
                    int palacey = hy.RNG.Next(centery - 2, centery + 3);
                    while (map[palacey, palacex] != Terrain.MOUNAIN)
                    {
                        palacex += delta;
                    }
                    map[palacey, palacex] = Terrain.PALACE;
                    palace3.Ypos = palacey + 30;
                    palace3.Xpos = palacex;
                    map[palacey, palacex + delta] = Terrain.MOUNAIN;

                }
            }
            return true;
        }
        private void PlaceRocks()
        {
            int rockNum = hy.RNG.Next(3);
            int cavePicked = 0;
            while (rockNum > 0)
            {
                List<Location> Caves = Locations[Terrain.CAVE];
                Location cave = Caves[hy.RNG.Next(Caves.Count)];
                int caveConn = 0;
                if(caveConn != 0 && connections.ContainsKey(GetLocationByMem(cavePicked)))
                {
                    caveConn = connections[GetLocationByMem(cavePicked)].MemAddress;
                }
                if (hy.Props.boulderBlockConnections && cave.MemAddress != cavePicked && cave.MemAddress != caveConn)
                {
                    if (map[cave.Ypos - 30, cave.Xpos - 1] != Terrain.MOUNAIN && cave.Xpos + 2 < MAP_COLS && GetLocationByCoords(Tuple.Create(cave.Ypos - 30, cave.Xpos + 2)) == null)
                    {
                        map[cave.Ypos - 30, cave.Xpos - 1] = Terrain.ROCK;
                        map[cave.Ypos - 30, cave.Xpos] = Terrain.ROAD;
                        map[cave.Ypos - 30, cave.Xpos + 1] = Terrain.CAVE;
                        if (cave.Xpos + 2 < MAP_COLS)
                        {
                            map[cave.Ypos - 30, cave.Xpos + 2] = Terrain.MOUNAIN;
                        }
                        cave.Xpos++;
                        rockNum--;
                    }
                    else if (map[cave.Ypos - 30, cave.Xpos + 1] != Terrain.MOUNAIN && cave.Xpos - 2 > 0 && GetLocationByCoords(Tuple.Create(cave.Ypos - 30, cave.Xpos - 2)) == null)
                    {
                        map[cave.Ypos - 30, cave.Xpos + 1] = Terrain.ROCK;
                        map[cave.Ypos - 30, cave.Xpos] = Terrain.ROAD;
                        map[cave.Ypos - 30, cave.Xpos - 1] = Terrain.CAVE;
                        if (cave.Xpos - 2 >= 0)
                        {
                            map[cave.Ypos - 30, cave.Xpos - 2] = Terrain.MOUNAIN;
                        }
                        cave.Xpos--;
                        rockNum--;
                    }
                    else if (map[cave.Ypos - 29, cave.Xpos] != Terrain.MOUNAIN && cave.Ypos - 32 < MAP_COLS && GetLocationByCoords(Tuple.Create(cave.Ypos - 32, cave.Xpos)) == null)
                    {
                        map[cave.Ypos - 29, cave.Xpos] = Terrain.ROCK;
                        map[cave.Ypos - 30, cave.Xpos] = Terrain.ROAD;
                        map[cave.Ypos - 31, cave.Xpos] = Terrain.CAVE;
                        if (cave.Ypos - 32 >= 0)
                        {
                            map[cave.Ypos - 32, cave.Xpos] = Terrain.MOUNAIN;
                        }
                        cave.Ypos--;
                        rockNum--;
                    }
                    else if (map[cave.Ypos - 31, cave.Xpos] != Terrain.MOUNAIN && cave.Ypos - 28 < MAP_COLS && GetLocationByCoords(Tuple.Create(cave.Ypos - 28, cave.Xpos)) == null)
                    {
                        map[cave.Ypos - 31, cave.Xpos] = Terrain.ROCK;
                        map[cave.Ypos - 30, cave.Xpos] = Terrain.ROAD;
                        map[cave.Ypos - 29, cave.Xpos] = Terrain.CAVE;
                        if (cave.Ypos - 28 < MAP_ROWS)
                        {
                            map[cave.Ypos - 28, cave.Xpos] = Terrain.MOUNAIN;
                        }
                        cave.Ypos++;
                        rockNum--;
                    }
                    cavePicked = cave.MemAddress;
                }
                else if (!connections.Keys.Contains(cave) && cave != cave1 && cave != cave2 && cave.MemAddress != cavePicked)
                {
                    if (map[cave.Ypos - 30, cave.Xpos - 1] != Terrain.MOUNAIN)
                    {
                        map[cave.Ypos - 30, cave.Xpos - 1] = Terrain.ROCK;
                        rockNum--;
                    }
                    else if (map[cave.Ypos - 30, cave.Xpos + 1] != Terrain.MOUNAIN)
                    {
                        map[cave.Ypos - 30, cave.Xpos + 1] = Terrain.ROCK;
                        rockNum--;
                    }
                    else if (map[cave.Ypos - 29, cave.Xpos] != Terrain.MOUNAIN)
                    {
                        map[cave.Ypos - 29, cave.Xpos] = Terrain.ROCK;
                        rockNum--;
                    }
                    else if (map[cave.Ypos - 31, cave.Xpos] != Terrain.MOUNAIN)
                    {
                        map[cave.Ypos - 31, cave.Xpos] = Terrain.ROCK;
                        rockNum--;
                    }
                    cavePicked = cave.MemAddress;

                }
            }
        }

      

        

        private void drawMountains()
        {
            //create some mountains
            int mounty = hy.RNG.Next(22, 42);
            map[mounty, 0] = Terrain.MOUNAIN;
            bool placedRoad = false;


            int endmounty = hy.RNG.Next(22, 42);
            int endmountx = hy.RNG.Next(2, 8);
            int x2 = 0;
            int y2 = mounty;
            int placedRocks = 0;
            while (x2 != (MAP_COLS - endmountx) || y2 != endmounty)
            {
                if (Math.Abs(x2 - (MAP_COLS - endmountx)) >= Math.Abs(y2 - endmounty))
                {
                    if (x2 > MAP_COLS - endmountx && x2 > 0)
                    {
                        x2--;
                    }
                    else if (x2 < MAP_COLS - 1)
                    {
                        x2++;
                    }
                }
                else
                {
                    if (y2 > endmounty && y2 > 0)
                    {
                        y2--;
                    }
                    else if (y2 < MAP_ROWS - 1)
                    {
                        y2++;
                    }
                }
                if (x2 != MAP_COLS - endmountx || y2 != endmounty)
                {
                    if (map[y2, x2] == Terrain.NONE)
                    {
                        map[y2, x2] = Terrain.MOUNAIN;
                    }
                    else
                    {
                        if (!placedRoad && map[y2, x2 + 1] != Terrain.ROAD)
                        {
                            if (hy.RNG.NextDouble() > .5 && (x2 > 0 && map[y2, x2 - 1] != Terrain.ROCK) && (x2 < MAP_COLS - 1 && map[y2, x2 + 1] != Terrain.ROCK) && (((y2 > 0 && map[y2 - 1, x2] == Terrain.ROAD) && (y2 < MAP_ROWS - 1 && map[y2 + 1, x2] == Terrain.ROAD)) || ((x2 > 0 && map[y2, x2 - 0] == Terrain.ROAD) && (x2 < MAP_COLS - 1 && map[y2, x2 + 1] == Terrain.ROAD))))
                            {
                                Location roadEnc = GetLocationByMem(0x4636);
                                roadEnc.Xpos = x2;
                                roadEnc.Ypos = y2 + 30;
                                roadEnc.CanShuffle = false;
                                roadEnc.Reachable = true;
                                placedRoad = true;
                            }
                            else if (placedRocks < 1)
                            {
                                Location roadEnc = GetLocationByMem(0x4636);
                                if ((roadEnc.Ypos - 30 != y2 && roadEnc.Xpos - 1 != x2) && (roadEnc.Ypos - 30 + 1 != y2 && roadEnc.Xpos != x2) && (roadEnc.Ypos - 30 - 1 != y2 && roadEnc.Xpos != x2) && (roadEnc.Ypos - 30 != y2 && roadEnc.Xpos + 1 != x2))
                                {
                                    map[y2, x2] = Terrain.ROCK;
                                    placedRocks++;
                                }
                            }
                        }
                        else if (placedRocks < 1)
                        {

                            map[y2, x2] = Terrain.ROCK;
                            placedRocks++;
                        }
                    }
                }
            }

            if (!placedRoad)
            {
                Location roadEnc = GetLocationByMem(0x4636);
                roadEnc.Xpos = 0;
                roadEnc.Ypos = 0;
                roadEnc.CanShuffle = false;
            }
        }


        public void updateVisit()
        {
            UpdateReachable();

            foreach (Location location in AllLocations)
            {
                if (location.Ypos > 30)
                {
                    if (v[location.Ypos - 30, location.Xpos])
                    {
                        location.Reachable = true;
                        if (connections.Keys.Contains(location))
                        {
                            Location l2 = connections[location];
                            if ((location.NeedBagu && (bagu.Reachable || hy.SpellGet[Spell.fairy])))
                            {
                                l2.Reachable = true;
                                v[l2.Ypos - 30, l2.Xpos] = true;
                            }

                            if (location.NeedFairy && hy.SpellGet[Spell.fairy])
                            {
                                l2.Reachable = true;
                                v[l2.Ypos - 30, l2.Xpos] = true;
                            }

                            if (location.NeedJump && (hy.SpellGet[Spell.jump] || hy.SpellGet[Spell.fairy]))
                            {
                                l2.Reachable = true;
                                v[l2.Ypos - 30, l2.Xpos] = true;
                            }

                            if (!location.NeedFairy && !location.NeedBagu && !location.NeedJump)
                            {
                                l2.Reachable = true;
                                v[l2.Ypos - 30, l2.Xpos] = true;
                            }
                        }
                    }
                }
            }
            if (lifeNorth.Reachable && lifeNorth.TownNum == Town.NEW_KASUTO)
            {
                lifeSouth.Reachable = true;
            }
        }

        
    }
}