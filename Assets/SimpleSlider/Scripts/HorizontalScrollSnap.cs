using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.SimpleSlider.Scripts
{
	/// <summary>
	/// Performs center/focus on child and swipe features.
	/// </summary>
	[RequireComponent(typeof(ScrollRect))]
	public class HorizontalScrollSnap : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public ScrollRect ScrollRect;
		public GameObject Pagination;
		[Header("Vertical Content")]
		public GameObject TextContainer;
		public ScrollRect TextScrollViewScrollRect;
		[Header("Setting")]
		public int SwipeThreshold = 50;
		public float SwipeTime = 0.5f;
		[Tooltip("In Percentage")]
		public float VerticalScrollOffset = 0.3f;
		[Tooltip("In Seconds")]
		public double SliderTimerInterval = 5d;
		[Tooltip("In Seconds")]
		public double TimeWaitToRestartTimerAfterInteractionEnd = 30d;

		private Toggle[] _pageToggles;

		private int _pageSize;
		private List<GameObject> _contentGOs = new List<GameObject>();
		private GridLayoutGroup _bannerGridLayout;

		private bool _drag;
		private bool _lerp;
		private int _page;
		private float _dragTime;

		private Timer _timer;
		private bool _shouldPerformSlide = false;
		private bool _isUserInteract = false;

		public void Initialize(int pageSize = 0)
		{
			ScrollRect.horizontalNormalizedPosition = 0;
			_bannerGridLayout = ScrollRect.content.GetComponent<GridLayoutGroup>();

			_pageToggles = Pagination.GetComponentsInChildren<Toggle>(true);
			_pageSize = pageSize;

			// Clear the list before insert
			_contentGOs.Clear();
			// Get total linked content size
			for (int i = 0; i < TextScrollViewScrollRect.content.childCount; i++)
			{
				if (TextScrollViewScrollRect.content.GetChild(i).CompareTag("NewEventDetailLinkedTextContent"))
					_contentGOs.Add(TextScrollViewScrollRect.content.GetChild(i).gameObject);
			}

			if (_timer == null)
			{
				_timer = new Timer(TimeSpan.FromSeconds(SliderTimerInterval).TotalMilliseconds);

				_timer.Elapsed += (sender, e) =>
				{
					_shouldPerformSlide = true;
					_isUserInteract = false;
					_lerp = true;
				};
				_timer.Start();
			}
			else RestartTimer();

			UpdatePaginator(_page);
			enabled = true;
		}

        /// <summary>
        /// Performs focusing on target page.
        /// </summary>
        public void Update()
		{
			// _bannerGridLayout.cellSize = new Vector2(Mathf.Lerp(_bannerGridLayout.cellSize.x, 600, 5 * Time.deltaTime), 600);

			if (!_lerp || _drag) return;

			PerformSlide();
		}

        private void OnDestroy()
        {
			_timer.Stop();

			_contentGOs = null;
			_timer = null;
        }

        /// <summary>
        /// Show next page.
        /// </summary>
        public void SlideNext()
		{
			Slide(1);
		}

		/// <summary>
		/// Show prev page.
		/// </summary>
		public void SlidePrev()
		{
			Slide(-1);
		}
		private void PerformSlide()
        {
			// Do not update the pagination
			/*
			if (Pagination)
			{
				var page = GetCurrentPage();

				if(page > -1 && page < _pageSize)
                {
					if (!_pageToggles[page].isOn)
					{
						UpdatePaginator(page);
					}
				}
			}
			*/
			// Auto Slide triggered
			if (_shouldPerformSlide)
            {
				// Reset the property 
				_shouldPerformSlide = false;
				// Perform slide to slider
				Slide(1, true);
            }
			// Manual slide by user
			else if(_isUserInteract)
            {
				if (_page > -1 && _page < _contentGOs.Count)
				{
					float textContentContainerHeight = TextScrollViewScrollRect.content.rect.height;
					Transform textContent = _contentGOs[_page].transform;
					// Use anchored position instedad of position when layout group existed in parent go
					float textContentPosY = Math.Abs(textContent.GetComponent<RectTransform>().anchoredPosition.y);
					// Debug.Log($"textContentPosY: {textContentPosY}, containerHeight: {textContentContainerHeight}, count: {_contentGOs.Count}");
					// Formula => (position Y + (Offset of position Y)) / container height
					float targetScrolledNormalizedPos = (textContentPosY + (textContentPosY * VerticalScrollOffset)) / textContentContainerHeight;
					TextScrollViewScrollRect.verticalNormalizedPosition = Mathf.Lerp(
						TextScrollViewScrollRect.verticalNormalizedPosition,
						(1 - targetScrolledNormalizedPos),
						5 * Time.deltaTime
					);
				}
			}
			var horizontalNormalizedPosition = (float)_page / (ScrollRect.content.childCount - 1);
			// Debug.Log($"vertNormPos: {TextScrollViewScrollRect.verticalNormalizedPosition}");
			ScrollRect.horizontalNormalizedPosition = Mathf.Lerp(ScrollRect.horizontalNormalizedPosition, horizontalNormalizedPosition, 5 * Time.deltaTime);

			if (Math.Abs(ScrollRect.horizontalNormalizedPosition - horizontalNormalizedPosition) < 0.001f)
			{
				ScrollRect.horizontalNormalizedPosition = horizontalNormalizedPosition;
				_lerp = false;
			}
		}
		private void Slide(int direction, bool fromTimer = false)
		{
			direction = Math.Sign(direction);
			// Stop sliding to the next/ prev slide
			if ((_page == 0 && direction == -1 || _page == ScrollRect.content.childCount - 1 && direction == 1) && !fromTimer) return;
			// Return to the first slide if triggered by timer
			else if (_page == ScrollRect.content.childCount - 1 && direction == 1) _page = 0;
			// continue
			else _page += direction;
			Debug.Log($"Slide called, fromTimer: {fromTimer}, page: {_page}");
			_lerp = true;
		}

		private int GetCurrentPage()
		{
			return Mathf.RoundToInt(ScrollRect.horizontalNormalizedPosition * (ScrollRect.content.childCount - 1));
		}

		private void UpdatePaginator(int page)
		{
			if (Pagination)
			{
				if (page > -1 && page < _pageSize)
				{
					// _pageToggles[page].isOn = true;
				}
			}
		}

		private void RestartTimer()
        {
			_timer.Stop();
			// Delay 1 seconds before timer starts
			StartTimer(1);
        }
		private void StartTimer(double timeDelay)
		{
			_shouldPerformSlide = false;
			// Delay x amount second before starting the timer
			Task.Delay(Mathf.CeilToInt((float)TimeSpan.FromSeconds(timeDelay).TotalMilliseconds))
				.ContinueWith(_ => { _timer.Start(); });
		}
		public void OnBeginDrag(PointerEventData eventData)
		{
			_drag = true;
			_dragTime = Time.time;
			// Stop timer when start to drag
			_timer.Stop();
		}

		public void OnDrag(PointerEventData eventData)
		{
			var page = GetCurrentPage();

			if (page != _page)
			{
				_page = page;
				UpdatePaginator(page);
			}
		}
		
		public void OnEndDrag(PointerEventData eventData)
		{
			var delta = eventData.pressPosition.x - eventData.position.x;

			if (Mathf.Abs(delta) > SwipeThreshold && Time.time - _dragTime < SwipeTime)
			{
				var direction = Math.Sign(delta);

				Slide(direction);
			}

			StartTimer(TimeWaitToRestartTimerAfterInteractionEnd);

			_drag = false;
			_lerp = true;
			_isUserInteract = true;
		}
	}
}