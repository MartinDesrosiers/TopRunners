public static class ObjectRotation {

	public static int GetObectRotation(string name) {
		int rotation;
		name = name.Replace("(Clone)", "");

		switch(name) {
			case "GroundSpikes":
				rotation = 90;
			break;

			case "Button":
				rotation = 90;
			break;

			case "ArrowGuide":
				rotation = 45;
			break;
			
			case "Laser":
				rotation = 45;
			break;
			
			case "Jump":
				rotation = 45;
			break;
			
			case "Fireball":
				rotation = 45;
			break;

			case "AccelerationPad":
				rotation = -1;
			break;

			case "Canon":
				rotation = -1;
			break;
			
			case "Door":
				rotation = -1;
			break;
			
			case "SpawnPoint":
				rotation = -1;
			break;
			
			case "Ramp1x1":
				rotation = -1;
			break;
			
			case "Ramp2x1":
				rotation = -1;
			break;

			case "Ground_Cool":
				rotation = -1;
			break;

			case "Air_Bird":
				rotation = -1;
			break;

			default:
				rotation = 0;
			break;
		}

		return rotation;
	}
}