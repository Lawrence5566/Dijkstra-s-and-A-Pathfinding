using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;
namespace Pathfinder
{
    class AStar
    {
        public bool[,] closed, inPath; //closed locations and wether location is final path
        public float[,] cost;  //cost for each location
        public Coord2[,] link; //link for each location = coords of neighbour location

        public AStar()
        {
            closed = new bool[40, 40];
            cost = new float[40, 40];
            link = new Coord2[40, 40];
            inPath = new bool[40, 40];
        }

        public void Build(Level level, AiBotBase bot, Player plr)
        {
            for (int i = 0; i < 40; i++)
            {
                for (int n = 0; n < 40; n++)
                {
                    closed[i, n] = false;
                    inPath[i, n] = false;
                    cost[i, n] = 100000;
                    link[i, n] = new Coord2(-1, -1);
                }
            }

            closed[bot.GridPosition.X, bot.GridPosition.Y] = false;
            cost[bot.GridPosition.X, bot.GridPosition.Y] = 0;
            int x = 0;
            int y = 0;
            while (closed[plr.GridPosition.X, plr.GridPosition.Y] == false)
            { //while player location is open 
                float minValue = float.PositiveInfinity;
                int minFirstIndex = -1;
                int minSecondIndex = -1;

                for (int i = 40 - 1; i >= 0; --i)
                {
                    for (int j = 40 - 1; j >= 0; --j)
                    {
                        int Xdiff = Math.Abs(plr.GridPosition.X - i);
                        int Ydiff = Math.Abs(plr.GridPosition.Y - j);

                        float value = cost[i, j] + Xdiff + Ydiff;

                        if (value < minValue && !closed[i, j] && level.ValidPosition(new Coord2(i, j)))
                        { //if its smaller, and not closed, and not blocked
                            minFirstIndex = i;
                            minSecondIndex = j;

                            minValue = value;
                        }
                    }
                }
                x = minFirstIndex;
                y = minSecondIndex;

                closed[x, y] = true; //mark lowest cost location as closed   

                for (int a = -1; a < 2; a++)
                {
                    for (int b = -1; b < 2; b++)
                    {
                        if (level.ValidPosition(new Coord2(x + a, y + b)) && cost[x, y] + 1 < cost[x + a, y + b] && !closed[x + a, y + b])
                        {
                            float costl;
                            if (a + b == 0 || a + b == 2)
                            {
                                costl = 1.4f;
                            }
                            else
                            {
                                costl = 1f;
                            }                  

                            cost[x + a, y + b] = cost[x, y] + costl;
                            link[x + a, y + b] = new Coord2(x, y);
                        }

                    }
                }

            }


            bool done = false; //sets to true when player's location is found
            Coord2 nextClosed = plr.GridPosition; //start path

            while (!done)
            {
                inPath[nextClosed.X, nextClosed.Y] = true;
                nextClosed = link[nextClosed.X, nextClosed.Y];
                if (nextClosed == bot.GridPosition) done = true;
            }

        }

    }
}