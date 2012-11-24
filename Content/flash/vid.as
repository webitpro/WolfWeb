package {
	import com.greensock.TweenLite;
	import com.greensock.plugins.AutoAlphaPlugin;
	import com.greensock.plugins.TweenPlugin;

	import fl.video.FLVPlayback;
	import fl.video.VideoEvent;
	import fl.video.VideoScaleMode;

	import flash.display.LoaderInfo;
	import flash.display.MovieClip;
	import flash.display.Sprite;
	import flash.events.MouseEvent;
	import flash.events.TimerEvent;
	import flash.external.ExternalInterface;
	import flash.geom.Point;
	import flash.geom.Rectangle;
	import flash.media.Video;
	import flash.text.TextField;
	import flash.utils.Timer;

	public class vid extends Sprite {
		//Timeline objects
		public var vidObj:FLVPlayback;
		public var mouseCapture:Sprite;
		public var controlBar:MovieClip;
		public var output:TextField;
		public var bigPlay:Sprite;

		private var isExt:Boolean;
		private var playing:Boolean = false;
		private var muted:Boolean = false;
		private var vidPerc:Number = 0;

		public function vid() {
			TweenPlugin.activate([AutoAlphaPlugin]);

			//Prep actions
			controlBar.playPause.buttonMode = true;
			controlBar.mute.buttonMode = true;
			controlBar.playPause.stop();
			controlBar.mute.gotoAndStop("unmute");
			controlBar.progress.width = 0;
			controlBar.progress.mouseEnabled = false;
			bigPlay.alpha = 0;
			bigPlay.visible = false;
			bigPlay.buttonMode = true;
			output.visible = false;

			//output.appendText("Playing\n");

			//Read any flashvars
			var varName:String;
			var paramObj:Object = LoaderInfo(this.root.loaderInfo).parameters;

			ExternalInterface.addCallback("stop", stopVid);

			if (paramObj.path && paramObj.path.toString().length > 0) {
				//output.appendText("FlashVar, loading path");
				vidObj.source = paramObj.path;
			} else {
				vidObj.source = "test.flv";
			}

			controlBar.alpha = 0;

			vidObj.addEventListener(VideoEvent.PLAYHEAD_UPDATE, updateUI);
			vidObj.addEventListener(VideoEvent.PLAYING_STATE_ENTERED, playStarted);
			mouseCapture.addEventListener(MouseEvent.MOUSE_MOVE, mouseShifting);
			mouseCapture.addEventListener(MouseEvent.CLICK, switchPlaying);
			controlBar.playPause.addEventListener(MouseEvent.CLICK, switchPlaying);
			controlBar.mute.addEventListener(MouseEvent.CLICK, switchMute);
			controlBar.handle.addEventListener(MouseEvent.MOUSE_DOWN, dragHandle);
			controlBar.track.addEventListener(MouseEvent.MOUSE_DOWN, dragHandle);
			bigPlay.addEventListener(MouseEvent.CLICK, switchPlaying);
		}

		/*Event Handlers*/

		private function updateUI(evt:VideoEvent):void {
			vidPerc = evt.playheadTime / vidObj.totalTime;
			//trace(vidPerc);
			var theTarget:Number = 51 + (628 * vidPerc);
			TweenLite.to(controlBar.handle, 0.2, {x: theTarget});
			TweenLite.to(controlBar.progress, 0.2, {width: theTarget - 51});
		}

		private function mouseShifting(evt:MouseEvent):void {
			TweenLite.to(controlBar, 0.2, {alpha: 1});
			TweenLite.killDelayedCallsTo(hideIt);
			TweenLite.delayedCall(3, hideIt);
		}

		private function playStarted(evt:VideoEvent):void {
			setPlaying(true);
			playing = true;
		}

		private function hideIt():void {
			TweenLite.killTweensOf(controlBar);
			TweenLite.to(controlBar, 0.5, {alpha: 0});
		}

		private function dragHandle(evt:MouseEvent):void {
			vidObj.pause();
			controlBar.handle.startDrag(true, new Rectangle(51, 13, 628, 0));
			addEventListener(MouseEvent.MOUSE_UP, dropHandle);
			TweenLite.killDelayedCallsTo(hideIt);
		}

		private function dropHandle(evt:MouseEvent):void {
			removeEventListener(MouseEvent.MOUSE_UP, dropHandle);
			stopDrag();
			controlBar.progress.width = controlBar.handle.x - 51;
			var vidPerc = (controlBar.handle.x - 51) / 628;
			vidObj.seekPercent(vidPerc * 100);
			vidObj.play();
		}

		private function switchPlaying(evt:MouseEvent):void {
			setPlaying(!playing);
		}

		private function switchMute(evt:MouseEvent):void {
			setMuted(!muted);
		}

		//External call available to JS
		public function stopVid():void {
			setPlaying(false);
			vidObj.stop();
		}

		/**
		 * Set interface state to playing or paused.
		 * @param isPlaying True if video is playing, false if it is paused
		 *
		 */
		private function setPlaying(isPlaying:Boolean):void {
			playing = isPlaying;
			if (isPlaying) {
				controlBar.playPause.gotoAndStop("pause");
				vidObj.play();
				TweenLite.to(bigPlay, 0.2, {autoAlpha: 0});
			} else {
				controlBar.playPause.gotoAndStop("play");
				vidObj.pause();
				TweenLite.to(bigPlay, 0.2, {autoAlpha: 1});
			}
		}

		private function setMuted(isMuted:Boolean):void {
			muted = isMuted;
			if (isMuted) {
				vidObj.volume = 0;
				controlBar.mute.gotoAndStop("mute");
			} else {
				vidObj.volume = 1;
				controlBar.mute.gotoAndStop("unmute");
			}
		}
	}
}
