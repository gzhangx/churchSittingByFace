using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WpfFaceApp
{
    public class CellInfo
    {
        public int x { get; set; }
        public int y { get; set; }
        public char type { get; set; }
        public String occupyedBy { get; set; }
        public String occupyedById { get; set; }
    }

    public class BlockInfo
    {
        public int Section { get; set; }
        public int MaxColumns { get; set; }
        public List<CellInfo[]> rows { get; set; }
    }
    public class BlockParser
    {
        public List<BlockInfo> blocks { get; set; }
        public BlockParser()
        {
            blocks = new List<BlockInfo>();
            for (int i = 0; i < 4; i++) blocks.Add(new BlockInfo { Section = i, rows = new List<CellInfo[]>() });
            var assembly = Assembly.GetExecutingAssembly();
            //String [] resources = assembly.GetManifestResourceNames();
            //Console.WriteLine(resources);
            using (var sr = new StreamReader(assembly.GetManifestResourceStream("WpfFaceApp.resources.seatConfig.txt")))
            {
                String content = sr.ReadToEnd();
                var lines = content.Split('\n');
                int curBlk = -1;
                int curBlkSize = 0;
                String curLine = lines[0];
                for (int i = 0; i < curLine.Length; i++)
                {
                    char c = curLine[i];
                    if (c == 'R')
                    {
                        if (curBlk >= 0)
                        {
                            blocks[curBlk].MaxColumns = curBlkSize;
                        }
                        curBlk++;
                        curBlkSize = 0;
                    }else
                    {
                        curBlkSize++;
                    }
                }
                

                for (int lineAt = 1; lineAt < lines.Length; lineAt++)
                {
                    curLine = lines[lineAt];
                    curBlk = -1;
                    CellInfo[] curCellLine = null;
                    int curCellId = 0;
                    for (int i = 0; i <curLine.Length;i++)
                    {
                        char c = curLine[i];
                        switch (c)
                        {
                            case 'X':
                            case 'N':
                            case ' ':
                                curCellLine[curCellId++] = new CellInfo { x = curCellId, y = lineAt, type = c, };
                                break;
                            default:
                                curBlk++;
                                if (curBlk < blocks.Count)
                                {
                                    var blk = blocks[curBlk];
                                    curCellLine = new CellInfo[blk.MaxColumns];
                                    blk.rows.Add(curCellLine);
                                    curCellId = 0;
                                }
                                else
                                {
                                    curCellLine = null;
                                }
                                break;
                        }
                    }
                }                
            }
        }
    }
}
