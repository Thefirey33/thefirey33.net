package net.thefirey33.thefirey33Fireserver.handlers;

import net.thefirey33.thefirey33Fireserver.Thefirey33Fireserver;

public abstract class BaseHandler implements Runnable {
    /**
     * The reference to the current fireServer plugin.
     */
    protected Thefirey33Fireserver fireServer;

    /**
     * The creation of this handler.
     * @param fireServer the fireServer plugin.
     */
    public BaseHandler(Thefirey33Fireserver fireServer) {
        this.fireServer = fireServer;
    }

    /**
     * When the method is ticked.
     */
    @Override
    public abstract void run();
}
