using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDController : MonoBehaviour {

	public GameObject healthBar;
	public GameObject maxHealthBar;

	public Image lowHealth;
	public Sprite selected;
	public Sprite mouse;
	public Sprite space;
	public Font font;

	public GameObject map;
	private GameObject inventory;

	private Health playerHealth;
	private int startingMaxHealth;

	private Vector2 resolution;

	private bool paused = false;

	// Use this for initialization
	void Start () {
		this.playerHealth = PlayerController.instance.Health;
		startingMaxHealth = this.playerHealth.maxHealth;

		this.inventory = new GameObject ();
		this.inventory.transform.parent = this.gameObject.transform;

		this.resolution = this.GetComponent<CanvasScaler> ().referenceResolution;
		this.resolution.y -= 3;

		if (this.font == null) {
			this.font = Resources.GetBuiltinResource<Font> ("Arial.ttf");
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Health bar
		healthBar.transform.localScale = new Vector3(1f * this.playerHealth.CurrentHealth / this.startingMaxHealth, 1f, 1f);
		maxHealthBar.transform.localScale = new Vector3(1f * this.playerHealth.maxHealth / this.startingMaxHealth, 1f, 1f);

		// low health warning
		if (this.playerHealth.CurrentHealth <= this.playerHealth.maxHealth / 10) {
			lowHealth.color = new Color(255, 0, 0, 0.3f);
		} else {
			lowHealth.color = new Color(0, 0, 0, 0);
		}

		// pause
		if (Input.GetKeyDown (KeyCode.Escape)) {
			this.paused = !this.paused;

			if (this.paused) {
				Time.timeScale = 0;
			} else {
				Time.timeScale = 1;
			}
		}

		if (this.paused) {
			lowHealth.color = new Color(0, 0, 0, 0.8f);
		}

		
		if (PlayerController.instance.inventory.Count > 0) {

			// inventory
			Destroy (this.inventory);
			this.inventory = new GameObject ();
			this.inventory.transform.parent = this.gameObject.transform;

			var sample = new GameObject ();
			sample.AddComponent<Image> ().sprite = this.selected;
			float iconWidth = sample.GetComponent<RectTransform> ().rect.width;
			Destroy (sample);

			float barWidth = iconWidth * PlayerController.instance.inventory.Count;

			int i = 0;

			foreach (var item in PlayerController.instance.inventory) {
				var icon = new GameObject ();
				icon.AddComponent<Image> ().sprite = item.sprite;
				var rectTransform = icon.GetComponent<RectTransform> ();
				icon.transform.SetParent(this.inventory.transform);
				float x = this.resolution.x - barWidth + i * rectTransform.rect.width;
				icon.transform.position = new Vector3 (x, this.resolution.y - rectTransform.rect.height / 2, 0);

				i++;
			}

			var border = new GameObject ();
			border.AddComponent<Image> ().sprite = this.selected;
			border.transform.SetParent (this.inventory.transform);
			var rt = border.GetComponent<RectTransform> ();

			var selX = this.resolution.x - barWidth + PlayerController.instance.InventoryIndex * rt.rect.width;
			var selY = this.resolution.y - rt.rect.height / 2;

			border.transform.position = new Vector3 (selX, selY, 0);

			var control = new GameObject ();

			switch (PlayerController.instance.inventory[PlayerController.instance.InventoryIndex].control) {
				case PlayerController.Control.MOUSE:
					control.AddComponent<Image> ().sprite = this.mouse;
				break;
				case PlayerController.Control.SPACE:
					control.AddComponent<Image> ().sprite = this.space;
				break;
			}
			var count = new GameObject ();

			switch(PlayerController.instance.inventory[PlayerController.instance.InventoryIndex].name){
				case "BowAndArrow":
					var arrowCount = count.AddComponent<Text> ();
					arrowCount.text = "" + PlayerController.instance.arrows;
					arrowCount.font = this.font;
					arrowCount.fontSize = 32;
				break;
				case "Rifle":
					var rifleCount = count.AddComponent<Text> ();
					rifleCount.text = "" + PlayerController.instance.bullets;
					rifleCount.font = this.font;
					rifleCount.fontSize = 32;
				break;
			}

			var outline = count.AddComponent<Outline>();
			control.transform.SetParent (this.inventory.transform);
			control.transform.position = new Vector3 (selX, selY, 0);

			count.transform.SetParent (this.inventory.transform);
			count.transform.position = new Vector3 (selX, selY, 0);
		}

		for (int i = 0; i < PlayerController.instance.inventory.Count - 1; i++) {
			if(PlayerController.instance.inventory[i].name == "BowAndArrow"){
				//show the ammount of arrows the player currently has

			}
			if(PlayerController.instance.inventory[i].name == "Rifle"){
				//show the amount of bullets the play currently has
			}
		}
	}
}
