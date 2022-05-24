﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OfficeOpenXml;

public class ExcelTable
{
    public bool importRichText;

    private Dictionary <int, Dictionary<int, ExcelTableCell>> cells = new Dictionary<int, Dictionary<int, ExcelTableCell>>();

    public string TableName;
    public int NumberOfRows;
    public int NumberOfColumns;

    public Vector2 Position;

    public ExcelTable()
    {

    }

    public ExcelTable(ExcelWorksheet sheet,bool importRichText)
    {
        TableName = sheet.Name;
        this.importRichText = importRichText;
        if (sheet.Dimension != null)
        {
            NumberOfRows = sheet.Dimension.Rows;
            NumberOfColumns = sheet.Dimension.Columns;
        }
        else
        {
            //empty Sheet
            NumberOfRows = 0;
            NumberOfColumns = 0;
        }

        for (int row = 1; row <= NumberOfRows; row++)
        {
            for (int column = 1; column <= NumberOfColumns; column++)
            {
                string value = ""; //default value for empty cell
                if (sheet.Cells[row, column].Value != null)
                {
                    value = sheet.Cells[row, column].Value.ToString();
                }

                if (sheet.Cells[row, column].IsRichText && importRichText)
                {
                    #if !UNITY_IOS
                    var richText = sheet.Cells[row, column].RichText;
                    StringBuilder result = new StringBuilder();

                    for (int i = 0; i < richText.Count; i++)
                    {
                        Color color = new Color(richText[i].Color.R / 255f, richText[i].Color.G / 255f,
                            richText[i].Color.B / 255f);
                        string rgb = ColorUtility.ToHtmlStringRGB(color);
                        if (rgb != "000000")
                        {
                            result.Append($"<color=#{rgb}>");
                        }

                        if (richText[i].Bold)
                        {
                            result.Append($"<b>");
                        }

                        if (richText[i].Italic)
                        {
                            result.Append($"<i>");
                        }
                        result.Append(richText[i].Text);
                        
                        if (richText[i].Bold)
                        {
                            result.Append($"</b>");
                        }

                        if (richText[i].Italic)
                        {
                            result.Append($"</i>");
                        }
                        
                        if (rgb != "000000")
                        {
                            result.Append("</color>");
                        }
                    }
                    SetValue(row, column, result.ToString());
                    #endif
                }
                else
                {
                    SetValue(row, column, value);
                }
            }
        }
    }

    public ExcelTableCell SetValue(int row, int column, string value)
    {
		CorrectSize(row, column);
        if (!cells.ContainsKey(row))
        {
            cells[row] = new Dictionary<int, ExcelTableCell>();
        }
        if (cells[row].ContainsKey(column))
        {
            cells[row][column].Value = value;

            return cells[row][column];
        }
        else
        {
            ExcelTableCell cell = new ExcelTableCell(row, column, value);
            cells[row][column] = cell;
            return cell;
        }
    }

    public object GetValue(int row, int column)
    {
        ExcelTableCell cell = GetCell(row, column);
        if (cell != null)
        {
            return cell.Value;
        }
        else
        {
            return SetValue(row, column, "").Value;
        }
    }

    public ExcelTableCell GetCell(int row, int column)
    {
        if (cells.ContainsKey(row))
        {
            if (cells[row].ContainsKey(column))
            {
                return cells[row][column];
            }
        }
        return null;
    }

    public void CorrectSize(int row, int column)
    {
        NumberOfRows = Mathf.Max(row, NumberOfRows);
        NumberOfColumns = Mathf.Max(column, NumberOfColumns);
    }

    public void SetCellTypeRow(int rowIndex, ExcelTableCellType type)
    {
        for (int column = 1; column <= NumberOfColumns; column++)
        {
            ExcelTableCell cell = GetCell(rowIndex, column);
            if (cell != null)
            {
                cell.Type = type;
            }
        }
    }

    public void SetCellTypeColumn(int columnIndex, ExcelTableCellType type, List<string> values = null)
    {
        for (int row = 1; row <= NumberOfRows; row++)
        {
            ExcelTableCell cell = GetCell(row, columnIndex);
            if (cell != null)
            {
                cell.Type = type;
                if (values != null)
                {
                    cell.ValueSelected = values;
                }
            }
        }
    }

    public void ShowLog() {
        string msg = "";
        for (int row = 1; row <= NumberOfRows; row++)
        {
            for (int column = 1; column <= NumberOfColumns; column++)
            {
                msg += string.Format("{0} ", GetValue(row, column));
            }
            msg += "\n";
        }
        Debug.Log(msg);
    }


}
