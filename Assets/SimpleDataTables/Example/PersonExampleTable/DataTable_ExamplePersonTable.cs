/*
 * This is an automaticly generated class. 
 * DO NOT EDIT raw text
 */

using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "Simple Data Tables/Create Instance/ExamplePersonTable")]
public class DataTable_ExamplePersonTable: DataTable
{
	public override Color[] NewColumnColors { get; set; } = new Color[]{new Color(1f, 1f, 1f), new Color(1f, 0f, 0.854969f), new Color(0.09767064f, 0.6037736f, 0f), new Color(1f, 0f, 0f), new Color(0.3058473f, 0.745283f, 0.6872009f), new Color(1f, 1f, 1f)};
	public override int[] NewColumnWidths { get; set; } = {100, 60, 60, 60, 200, 60};

	public override void Add(string NewKey)
	{
		Rows.Add(new DataTableRow_ExamplePersonTable(){ Key = NewKey });
	}
	public override FieldInfo[] GetVariableList(BindingFlags bindingFlags)
	{
		return typeof(DataTableRow_ExamplePersonTable).GetFields(bindingFlags);
	}
}
[System.Serializable]
public class DataTableRow_ExamplePersonTable: DataTableRow
{
	[SerializeField] public string Name;
	[SerializeField] public int Age;
	[SerializeField] public bool HasPet;
	[SerializeField] public Color HairColor;
}