import { fromStore, type Writable, writable } from 'svelte/store';

interface ToastOptions {
	message: string;
	show: boolean;
	time: number;
}

/**
 * The maximum amount of time that the toast will appear for.
 */
export const toastMaximumTime = 3;

/**
 * The toast's state.
 */
const currentToastState: Writable<ToastOptions> = writable({
	message: "Freeman. If you have knocked on the door, I would've let you in.",
	show: false,
	time: 0
});

/**
 * Default toast state export.
 */
export default currentToastState;

/**
 * Set the toast notification.
 * @param message The message to set.
 */
export function setToast(message: string) {
	const aniState = fromStore<ToastOptions>(currentToastState);

	// If there's already a toast instance rendering, ignore it.
	if (aniState.current.show) return;

	currentToastState.set({
		message: message,
		show: true,
		time: toastMaximumTime
	});

	let aniHandle = requestAnimationFrame(animateCountdown);
	let lastTime = 0;

	function animateCountdown(callback: number) {
		const animationDelta = (callback - lastTime) / 1000;

		if (lastTime != 0) {
			currentToastState.set({
				message: message,
				show: aniState.current.time > 0,
				time: aniState.current.time - animationDelta
			});
		}

		if (aniState.current.time < 0) {
			currentToastState.set({
				message: message,
				show: false,
				time: 0
			});
			cancelAnimationFrame(aniHandle);
			return;
		}
		aniHandle = requestAnimationFrame(animateCountdown);
		lastTime = callback;
	}
}
