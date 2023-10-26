from kivy.app import App
from kivy.lang import Builder
from kivy.core.text import LabelBase, DEFAULT_FONT
from kivy.resources import resource_add_path
from kivy.uix.boxlayout import BoxLayout
from kivy.uix.spinner import Spinner
from kivy.core.window import Window

class MainScreen(BoxLayout):
    def __init__(self, **kwargs):
        super(MainScreen, self).__init__(**kwargs)

class TagAlbumSpinner(Spinner):
    pass

class ListingShowSpinner(Spinner):
    pass

class SortSpinner(Spinner):
    pass

class SortTypeSpinner(Spinner):
    pass

class ViewerApp(App):
    def __init__(self, **kwargs):
        super(ViewerApp, self).__init__(**kwargs)
        
    def build(self):
        return MainScreen()

if __name__ == "__main__":
    resource_add_path('./fonts')
    LabelBase.register(DEFAULT_FONT, 'ipaexg.ttf')
    Builder.load_file('./src/kv/main.kv')

    Window.fullscreen = False
    Window.size = (1024, 768)
    Window.pos = (0, 0)

    ViewerApp().run()