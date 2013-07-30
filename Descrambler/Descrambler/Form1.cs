using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Resources;
using Descrambler.Properties;

namespace Descrambler
{
  public partial class Form1 : Form
  {
    private string[,] letters = new string[4, 4];
    private string test = Resources.enable1;
    private string[] words;   

    public Form1()
    {
      InitializeComponent();
      words = test.Split(new string[] { "\r\n" }, StringSplitOptions.None);    
    }

    private void Button1_Click(object sender, EventArgs e)
    {
      letters[0,0] = txtBox00.Text;
      letters[0,1] = txtBox01.Text;
      letters[0,2] = txtBox02.Text;
      letters[0,3] = txtBox03.Text;
      
      letters[1,0] = txtBox10.Text;
      letters[1,1] = txtBox11.Text;
      letters[1,2] = txtBox12.Text;
      letters[1,3] = txtBox13.Text;
      
      letters[2,0] = txtBox20.Text;
      letters[2,1] = txtBox21.Text;
      letters[2,2] = txtBox22.Text;
      letters[2,3] = txtBox23.Text;
      
      letters[3,0] = txtBox30.Text;
      letters[3,1] = txtBox31.Text;
      letters[3,2] = txtBox32.Text;
      letters[3,3] = txtBox33.Text;

      listBox1.Items.Clear();

      buildWord(new List<Tile>(), 0, 0);
      buildWord(new List<Tile>(), 0, 1);
      buildWord(new List<Tile>(), 0, 2);
      buildWord(new List<Tile>(), 0, 3);

      buildWord(new List<Tile>(), 1, 0);
      buildWord(new List<Tile>(), 1, 1);
      buildWord(new List<Tile>(), 1, 2);
      buildWord(new List<Tile>(), 1, 3);

      buildWord(new List<Tile>(), 2, 0);
      buildWord(new List<Tile>(), 2, 1);
      buildWord(new List<Tile>(), 2, 2);
      buildWord(new List<Tile>(), 2, 3);

      buildWord(new List<Tile>(), 3, 0);
      buildWord(new List<Tile>(), 3, 1);
      buildWord(new List<Tile>(), 3, 2);
      buildWord(new List<Tile>(), 3, 3);
    }

    private void buildWord(List<Tile> tiles, int row, int col)
    {
      if (row > 3 || col > 3 || row < 0 || col < 0)
        return;

      foreach (Tile tile in tiles)
      {
        if (row == tile.row && col == tile.col)
          return;
      }

      Tile newTile = new Tile();
      newTile.letter = letters[row, col];
      newTile.row = row;
      newTile.col = col;
      tiles.Add(newTile);

      string word = "";
      foreach (Tile tile in tiles)
        word += tile.letter;

      System.Console.WriteLine(word + " " + row + " " + col);

      bool startsWithWord = false;
      foreach (string dictionaryWord in words)
      {
        if (word == dictionaryWord)
        {
          listBox1.Items.Add(word);
          Application.DoEvents();
        }

       if (dictionaryWord.StartsWith(word))
          startsWithWord = true;
      }

      if (!startsWithWord)
        return;

      List<int> rowList = new List<int>();
      rowList.Add(row - 1);
      rowList.Add(row);
      rowList.Add(row + 1);
      List<int> colList = new List<int>();
      colList.Add(col - 1);
      colList.Add(col);
      colList.Add(col + 1);

      foreach (int newRow in rowList)
      {
        foreach (int newCol in colList)
          buildWord(new List<Tile>(tiles), newRow, newCol);
      }
    }
  }
}
