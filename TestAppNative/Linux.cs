
#if Linux
    .TitleBar((a, w, wv) => HeaderBar.New()
                            .PackEnd(
                                ToggleButton.New()
                                .IconName("open-menu-symbolic")
                                .OnClicked(() => wv.Ref.GrabFocus())
                            ))
#endif
