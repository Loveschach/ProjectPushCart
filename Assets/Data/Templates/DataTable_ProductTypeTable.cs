/*
 * This is an automaticly generated class. 
 * DO NOT EDIT raw text
 */

using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "Simple Data Tables/Create Instance/ProductTypeTable")]
public class DataTable_ProductTypeTable: DataTable
{
	public override Color[] NewColumnColors { get; set; } = new Color[]{new Color(1f, 1f, 1f), new Color(0.9921569f, 0.6f, 0.03137255f), new Color(0.09767064f, 0.6037736f, 0f), new Color(0.6039216f, 0f, 0.4592783f), new Color(0.3773585f, 0f, 0.364374f)};
	public override int[] NewColumnWidths { get; set; } = {100, 100, 200, 60, 60};

	public override void Add(string NewKey)
	{
		Rows.Add(new DataTableRow_ProductTypeTable(){ Key = NewKey });
	}
	public override FieldInfo[] GetVariableList(BindingFlags bindingFlags)
	{
		return typeof(DataTableRow_ProductTypeTable).GetFields(bindingFlags);
	}
}
[System.Serializable]
public class DataTableRow_ProductTypeTable: DataTableRow
{
	[SerializeField] public ProductTypes Type;
	[SerializeField] public GameObject GameObject;
	[SerializeField] public int WidthUnits;
	[SerializeField] public int HeightUnits;

}