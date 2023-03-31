public class CharacterStatModel
{
	private int maxHp;
	private int hp;
	private int attack;
	private int defense;
	private float speed;

	public int MaxHp { get => maxHp; set => maxHp = value; }
	public int Hp { get => hp; set => hp = value; }
	public int Attack { get => attack; set => attack = value; }
	public int Defense { get => defense; set => defense = value; }
	public float Speed { get => speed; set => speed = value; }

	public CharacterStatModel(int maxHp, int attack, int defense, float speed)
	{
		MaxHp = maxHp;
		Hp = maxHp;
		Attack = attack;
		Defense = defense;
		Speed = speed;
	}

	public float GetHpAmount()
	{
		return (float)hp / (float)maxHp;
	}
}
