using UnityEngine;
using System.Collections;

public class MovieScript : MonoBehaviour {

	void OnLevelWasLoaded(int level) {
		if (level == 1) {
			Handheld.PlayFullScreenMovie ("IntroMovie.mov", Color.black,
		                              FullScreenMovieControlMode.CancelOnInput,
		                              FullScreenMovieScalingMode.Fill);
		}
	}
}
