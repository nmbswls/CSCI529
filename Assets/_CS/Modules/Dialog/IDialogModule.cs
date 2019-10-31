using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IDialogModule : IModule
{

	DialogBlock LoadDialog();
}

