from kivy.app import App
from kivy.lang import Builder
from kivy.core.text import LabelBase, DEFAULT_FONT
from kivy.resources import resource_add_path
from kivy.uix.boxlayout import BoxLayout
from kivy.uix.spinner import Spinner

class MainScreen(BoxLayout):
    def __init__(self, **kwargs):
        super(MainScreen, self).__init__(**kwargs)

class SortSpinner(Spinner):
    pass

class SortTypeSpinner(Spinner):
    pass

class ViewerApp(App):
    def build(self):
        return MainScreen()

if __name__ == "__main__":
    resource_add_path('./fonts')
    LabelBase.register(DEFAULT_FONT, 'ipaexg.ttf')
    
    Builder.load_file('./src/kv/main.kv')

    ViewerApp().run()