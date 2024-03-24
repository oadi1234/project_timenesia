using UnityEngine;

public interface IUserInterfacePanel
{

    GameObject gameObject { get; }

    public void SelectButton(int index);

    public void ToggleActive();

    public void Open();

    public void Close();
}
