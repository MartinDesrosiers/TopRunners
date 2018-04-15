using UnityEngine;
using UnityEngine.UI;

public class scrollHandler : MonoBehaviour {
    public void TranslateLeft() {
        if (this.GetComponent<ScrollRect>().horizontalNormalizedPosition > 0) {
            this.GetComponent<ScrollRect>().horizontalNormalizedPosition -= 0.1f;
        }
    }

    public void TranslateRight() {
        if (this.GetComponent<ScrollRect>().horizontalNormalizedPosition < 1) {
            this.GetComponent<ScrollRect>().horizontalNormalizedPosition += 0.1f;
        }
    }
}