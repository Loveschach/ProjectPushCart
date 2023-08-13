/*
 * This is an automaticly generated class. 
 * DO NOT EDIT raw text
 */

using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "Simple Data Tables/Create Instance/StoreInventory")]
public class DataTable_StoreInventory: DataTable
{
	public override Color[] NewColumnColors { get; set; } = new Color[]{new Color(1f, 1f, 1f), new Color(0.09767064f, 0.6037736f, 0f), new Color(0.04929941f, 0.2830189f, 0.004004977f), new Color(0.990566f, 0.5994907f, 0.03270739f), new Color(0f, 0.4553149f, 0.9528302f), new Color(0.6039216f, 0f, 0.1828886f), new Color(0.3667977f, 0f, 0.6039216f)};
	public override int[] NewColumnWidths { get; set; } = {100, 100, 200, 60, 200, 200, 60};

	public override void Add(string NewKey)
	{
		Rows.Add(new DataTableRow_StoreInventory(){ Key = NewKey });
	}
	public override FieldInfo[] GetVariableList(BindingFlags bindingFlags)
	{
		return typeof(DataTableRow_StoreInventory).GetFields(bindingFlags);
	}
}
[System.Serializable]
public class DataTableRow_StoreInventory: DataTableRow
{
	[SerializeField] public string Name;
	[SerializeField] public string Description;
	[SerializeField] public ProductTypes Type;
	[SerializeField] public Texture2D LabelTexture;
	[SerializeField] public Color ProductColor;
	[SerializeField] public float Odds;

}