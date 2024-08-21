using System;
using Godot;
using ProjGMTK.Scripts.DataBinding;

namespace ProjGMTK.Scripts;

public partial class Option : Control
{

	public class SetupArgs
	{
		public Func<int> GetCostFunc;
		public string ButtonText;
		public Action ButtonPressed;
		public int CoolDown;
	}

	public Label CostLabel;
	public Button Button;
	public ProgressBar ProgressBar;

	public string OptionName;
	public Func<int> GetCostFunc;
	public int CoolDown;
	public ObservableProperty<int> CoolDownCounter;
	public Action ButtonPressed;
    
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CostLabel = GetNode<Label>("HBoxContainer/Cost");
		Button = GetNode<Button>("HBoxContainer/Button");
		Button.Pressed += OnButtonPressed;
		ProgressBar = Button.GetNode<ProgressBar>("ProgressBar");
		
		CoolDownCounter = new ObservableProperty<int>(nameof(CoolDownCounter), this, 0);
		CoolDownCounter.DetailedValueChanged += OnCoolDownCounterChanged;
		CoolDownCounter.FireValueChangeEventsOnInit();
		// GameMgr.Instance.RegisterTick(this);
	}

	public override void _Process(double delta)
	{
		var cost = GetCostFunc();
		if (cost > 0)
		{
			CostLabel.Text = $"{GetCostFunc()} sc to";
			CostLabel.Show();
		}
		else
		{
			CostLabel.Hide();
		}
		Button.Disabled = cost > GameMgr.Instance.GameState.ScaleCount;
		if (CoolDownCounter.Value > 0)
		{
			CoolDownCounter.Value = Mathf.Max(0, (int)(CoolDownCounter.Value - delta * 1000));
		}
	}

	public void Setup(object o)
	{
		var args = (SetupArgs)o;
		OptionName = args.ButtonText;
		GetCostFunc = args.GetCostFunc;
		Button.Text = OptionName;
		ButtonPressed += args.ButtonPressed;
		CoolDown = args.CoolDown;
	}

	private void OnButtonPressed()
	{
		if (CoolDownCounter.Value <= 0)
		{
			ButtonPressed?.Invoke();
			CoolDownCounter.Value = CoolDown;
		}
	}

	private void OnCoolDownCounterChanged(object sender, ValueChangedEventDetailedArgs<int> valueChangedEventDetailedArgs)
	{
		if (valueChangedEventDetailedArgs.NewValue > 0)
		{
			ProgressBar.Value = (float)valueChangedEventDetailedArgs.NewValue / CoolDown * 100;
			Button.Text = $"{valueChangedEventDetailedArgs.NewValue / 1000f:F1} s";
			ProgressBar.Show();
		}
		else
		{
			ProgressBar.Value = 0;
			Button.Text = OptionName;
			ProgressBar.Hide();
		}
	}
}