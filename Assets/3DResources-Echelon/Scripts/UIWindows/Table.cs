using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

[ExecuteInEditMode]
public class Table : MonoBehaviour
{
    [Header("DEBUG Test Kits")]
    public bool debugTrigger = false;
    public Vector2Int testSize;
    [SerializeField]
    [ReadOnly] private Vector2Int tableSize;
    private HorizontalLayoutGroup horiLayout;
    
    private class Column {
        public Transform transform;
        public LayoutElement element;
        public VerticalLayoutGroup vertLayout;
        public class Row        {
            public Transform transform;
            public LayoutElement element;
        }
        public List<Row> rows;
    }
    private List<Column> columns;

    void Awake()
    {
        InitTable();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("columns: " + columns?.Count.ToString() + "\trows: " + columns[0]?.rows?.Count.ToString());
        tableSize = new Vector2Int(columns.Count, columns.Count > 0 ? columns[0].rows.Count : -1);
    }

    public Transform GetCell(int col, int row)
    {
        if(col >= tableSize.x || row >= tableSize.y)
            return null;

        Transform colRow = columns[col].rows[row].transform;
        if(colRow.childCount == 0)
        {
            GameObject cell = Instantiate(new GameObject());
            cell.transform.parent = colRow.transform;
            cell.name = "cell";
            return cell.transform;
        }
        else
        {
            return colRow.GetChild(0).transform;
        }

       // return columns[col].rows[row].transform;
    }

    public void ResizeTable(int colNum, int rowNum)
    {
        InitTable();
        if (colNum > columns.Count)
            AddColumns(colNum - columns.Count);
        else if (colNum < columns.Count)
            for (int i = columns.Count; i > colNum; i--)
                RemoveColumn(i-1);

        if (rowNum > columns[0].rows.Count)
            AddRows(rowNum - columns[0].rows.Count);
        else if (rowNum < columns[0].rows.Count)
            for (int i = columns[0].rows.Count; i > rowNum; i--)
                RemoveRow(i-1);
        tableSize = new Vector2Int(columns.Count, columns.Count > 0? columns[0].rows.Count : -1);
    }

    public void AddRows(int num)
    {
        GameObject row;
        foreach (Column column in columns)
        {
            if (column.rows == null)
                column.rows = new List<Column.Row>();

            for (int i = 0; i < num; i++)
            {
                row = Instantiate(new GameObject());
                row.transform.parent = column.transform;
                row.transform.SetAsLastSibling();
                column.rows.Add(InitRow(row));
            }
        }
        tableSize = new Vector2Int(columns.Count, columns.Count > 0 ? columns[0].rows.Count : -1);
        Debug.Log("ADD ROW\ncolumns: " + columns?.Count.ToString() + "\trows: " + columns[0]?.rows?.Count.ToString());
    }

    public void RemoveRow(int index)    {
        if (columns?.Count == 0 || index >= columns[0].rows.Count)
            return;
        foreach(Column column in columns)
        {
            DestroyImmediate(column.transform.GetChild(index).gameObject);
            column.rows.RemoveAt(index);
        }
        tableSize = new Vector2Int(columns.Count, columns.Count > 0 ? columns[0].rows.Count : -1);
        Debug.Log("REMOVE ROW index: " + index.ToString() + "\ncolumns: " + columns?.Count.ToString() + "\trows: " + columns[0]?.rows?.Count.ToString());
    }

    public void AddColumns(int num)
    {
        GameObject column;

        if (columns == null)
            columns = new List<Column>();

        for (int i = 0; i < num; i++)
        {
            column = Instantiate(new GameObject());
            column.transform.parent = this.transform;
            column.transform.SetAsLastSibling();
            columns.Add(InitColumn(column));
        }
        tableSize = new Vector2Int(columns.Count, columns.Count > 0 ? columns[0].rows.Count : -1);
        Debug.Log("ADD COLUMN\ncolumns: " + columns?.Count.ToString() + "\trows: " + columns[0]?.rows?.Count.ToString());
    }

    public void RemoveColumn(int index)
    {
        if (index >= columns.Count)
            return;
        DestroyImmediate(this.transform.GetChild(index).gameObject);
        columns.RemoveAt(index);
        tableSize = new Vector2Int(columns.Count, columns.Count > 0 ? columns[0].rows.Count : -1);
        Debug.Log("REMOVE COLUMN index: " + index.ToString() + "\ncolumns: " + columns?.Count.ToString() + "\trows: " + columns[0]?.rows?.Count.ToString());

    }

    private List<Column> InitTable()
    {
        if(columns == null)
        {
            horiLayout = GetOrAddComponent<HorizontalLayoutGroup>(this.gameObject);
            columns = new List<Column>();
            foreach (Transform child in this.transform)
                columns.Add(InitColumn(child.gameObject));
        }
        return columns;
    }

    private Column InitColumn(GameObject element)
    {
        Column temp = new Column();
        temp.transform = GetOrAddComponent<Transform>(element);
        temp.element = GetOrAddComponent<LayoutElement>(element);
        temp.vertLayout = GetOrAddComponent<VerticalLayoutGroup>(element);
        temp.rows = new List<Column.Row>();
        foreach (Transform child2 in element.transform)
        {
            temp.rows.Add(InitRow(child2.gameObject));
        }
        return temp;
    }


    private Column.Row InitRow(GameObject element)
    {
        Column.Row row = new Column.Row();
        row.transform = GetOrAddComponent<Transform>(element);
        row.element = GetOrAddComponent<LayoutElement>(element);
        return row;
    }

    private T GetOrAddComponent<T>(GameObject element) where T : Component
    {
        T component = element.GetComponent<T>();

        if (component == null)
            component = element.AddComponent<T>() as T;

        return component;
    }

    // Update is called once per frame
    void Update()
    {
        if (debugTrigger)
        {
            ResizeTable(testSize.x, testSize.y);
            debugTrigger = false;
        }
    }
}
