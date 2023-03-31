public class CharacterActionModel
{
	private int actionRange;
	private float actionCooldown;

	public int ActionRange { get => actionRange; }
	public float ActionCooldown { get => actionCooldown; }

	public CharacterActionModel(int actionRange, float actionCooldown)
	{
		this.actionRange = actionRange;
		this.actionCooldown = actionCooldown;
	}
}
