/*
 * This is an automaticly generated class. 
 * DO NOT EDIT raw text
 */

using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "Simple Data Tables/Create Instance/ExampleItemTable")]
public class DataTable_ExampleItemTable: DataTable
{
	public override Color[] NewColumnColors { get; set; } = new Color[]{new Color(1f, 1f, 1f), new Color(0.09767064f, 0.6037736f, 0f), new Color(1f, 0f, 0.854969f), new Color(1f, 0f, 0.854969f), new Color(0f, 0.09575987f, 1f), new Color(0.1706817f, 0f, 0.6415094f), new Color(0.3058473f, 0.745283f, 0.6872009f), new Color(1f, 0f, 0f), new Color(0.09767064f, 0.6037736f, 0f), new Color(1f, 0.9450981f, 0f)};
	public override int[] NewColumnWidths { get; set; } = {100, 60, 60, 60, 60, 60, 200, 60, 60, 90};

	public override void Add(string NewKey)
	{
		Rows.Add(new DataTableRow_ExampleItemTable(){ Key = NewKey });
	}
	public override FieldInfo[] GetVariableList(BindingFlags bindingFlags)
	{
		return typeof(DataTableRow_ExampleItemTable).GetFields(bindingFlags);
	}
}
[System.Serializable]
public class DataTableRow_ExampleItemTable: DataTableRow
{
	[SerializeField] public int ItemID;
	[SerializeField] public string DisplayName;
	[SerializeField] public string Description;
	[SerializeField] public GameObject Model;
	[SerializeField] public Texture2D InventorIcon;
	[SerializeField] public Color InventoryBackgroundColor;
	[SerializeField] public bool Stackable;
	[SerializeField] public int MaxStackSize;
	[SerializeField] public Vector3 DisplayRotation;

}