using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CRUD
{
    {
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //creo una variable global para reconocer la accion a realizar cuando quiera confirmar/cancelar los cambios
        public String btnAccionElegida = "";

        public MainWindow()
        {
            InitializeComponent();
            Mostrar_Datos();
        }

        private void Mostrar_Datos()
        {
            BD_TiendaEntities entidades = new BD_TiendaEntities();
            List<PRODUCTO> productos = entidades.PRODUCTO.ToList<PRODUCTO>();
            Listado.ItemsSource = productos;
            Listado.Items.Refresh();
            
            //el campo ID tiene que estar activado porque vamos a buscar siempre por ID
            TextoID.IsEnabled = true;
            //mientras que deshabilitamos los campos producto y precio y el segundo stackpanel que de momento no sirve
            TextoProducto.IsEnabled = false;
            TextoPrecio.IsEnabled = false;
            ConfirmStackPanel.IsEnabled = false;
        }

        //1.He creado un metodo buscar para refactorizar el codigo, de momento que voy a tener que buscar para mas metodos
        private void buscar()
        {
            BD_TiendaEntities entidades = new BD_TiendaEntities();

            //2.He puesto un try-catch para capturar el error que saltaba cuando el ID era null.
            try
            {
                PRODUCTO producto = entidades.PRODUCTO.ToList<PRODUCTO>().Where(c => c.ID == int.Parse(TextoID.Text)).FirstOrDefault<PRODUCTO>();

                if (producto == null)
                {
                    MessageBox.Show("No se encontró el producto");
                }
                else
                {
                    TextoProducto.Text = producto.NOMBRE;
                    TextoPrecio.Text = Convert.ToString(producto.PRECIO);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("No has introducido el ID correctamente.");
            }
        }
        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            buscar();
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            Limpiar_textbox();
            //a la variable global se le asigna su String
            btnAccionElegida = "agregar";
            //Cuando se clica el boton para agregar, el ID se inhabilita porque el ID del producto es autoincrementable
            //mientras que los textboxes nombre y precio se activan
            TextoID.IsEnabled = false;
            TextoProducto.IsEnabled = true;
            TextoPrecio.IsEnabled = true;
            //se inhabilita ademas el primer stackpanel (todos sus botones)
            AccionesStackPanel.IsEnabled = false;
            //y se habilita el segundo stackpanel (confirmar/cancelar)
            ConfirmStackPanel.IsEnabled = true;

            MessageBox.Show("Introduzca el nombre y el precio del producto a añadir.");
            //no se agrega el producto aqui, se agregara con clicando el boton de confirmar
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            //llamo al metodo de búsqueda si el campo tiene valor
            if (TextoID.Text != "")
            {
                buscar();
                btnAccionElegida = "editar";
                //mientras que los textboxes nombre y precio se habilitan
                TextoProducto.IsEnabled = true;
                TextoPrecio.IsEnabled = true;
                //se inhabilita ademas el primer stackpanel (todos sus botones)
                AccionesStackPanel.IsEnabled = false;
                //y se habilita el segundo stackpanel (confirmar/cancelar)
                ConfirmStackPanel.IsEnabled = true;
            }   
            else
                MessageBox.Show("Introduzca el ID del producto a buscar para editarlo.");
            
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            //llamo al metodo de búsqueda si el campo tiene valor
            if (TextoID.Text != "")
            {
                btnAccionElegida = "eliminar";
                //Cuando se clica el boton para eliminar, el ID se habilita para buscar el producto por su ID
                //mientras que los textboxes nombre y precio se deshabilitan
                TextoID.IsEnabled = true;
                TextoProducto.IsEnabled = false;
                TextoPrecio.IsEnabled = false;
                //se inhabilita ademas el primer stackpanel (todos sus botones)
                AccionesStackPanel.IsEnabled = false;
                //y se habilita el segundo stackpanel (confirmar/cancelar)
                ConfirmStackPanel.IsEnabled = true;
                //llamo al metodo de búsqueda
                buscar();
            }
            else
                MessageBox.Show("Introduzca el ID del producto a buscar para eliminarlo.");
        }

        private void Limpiar_textbox()
        {
            TextoProducto.Clear();
            TextoID.Clear();
            TextoPrecio.Clear();
        }

        private void BtnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            BD_TiendaEntities entidades = new BD_TiendaEntities();
            PRODUCTO p = new PRODUCTO();

            //boton agregar
            if (btnAccionElegida == "agregar")
            {
                if (TextoProducto.Text == "")
                {
                    MessageBox.Show("Tiene que completar los campos del producto");
                }
                else
                {
                    p.NOMBRE = TextoProducto.Text;
                    try
                    {
                        p.PRECIO = int.Parse(TextoPrecio.Text);
                        entidades.PRODUCTO.Add(p);
                        entidades.SaveChanges();
                        Mostrar_Datos();
                        MessageBox.Show("Producto Añadido");
                        Limpiar_textbox();
                        //Cuando anadimos el producto vuelvo a desactivar los textboxes de precio y producto
                        TextoPrecio.IsEnabled = false;
                        TextoProducto.IsEnabled = false;
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Formato de precio no válido");
                    }
                }
            }
            //boton editar
            else if (btnAccionElegida == "editar")
            {
                PRODUCTO producto = entidades.PRODUCTO.ToList<PRODUCTO>().Where(c => c.ID == int.Parse(TextoID.Text)).FirstOrDefault<PRODUCTO>();
                if (TextoProducto.Text == "")
                {
                    MessageBox.Show("Tiene que completar los campos del producto");
                }
                else
                {
                    try
                    {
                        producto.NOMBRE = TextoProducto.Text;
                        producto.PRECIO = int.Parse(TextoPrecio.Text);
                        entidades.SaveChanges();
                        Mostrar_Datos();
                        Limpiar_textbox();
                        MessageBox.Show("Cambios realizados correctamente");
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Formato de precio no válido");
                    }
                }
            }
            //boton eliminar
            else
            {
                try
                {
                    PRODUCTO producto = entidades.PRODUCTO.ToList<PRODUCTO>().Where(c => c.ID == int.Parse(TextoID.Text)).FirstOrDefault<PRODUCTO>();
                    entidades.PRODUCTO.Remove(producto);
                    entidades.SaveChanges();
                    Mostrar_Datos();
                    Limpiar_textbox();
                    MessageBox.Show("Producto Eliminado");
                }
                catch
                {
                    MessageBox.Show("Hubo algún problema en la eliminación del producto.");
                }
            }

            //independientemente de la accion escogida, se vuelve a habilitar el primer stackpanel
            AccionesStackPanel.IsEnabled = true;
            //y se inhabilita el segundo stackpanel (confirmar/cancelar)
            ConfirmStackPanel.IsEnabled = false;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            //se limpia todo
            Limpiar_textbox();
            //se inhabilitan de nuevo los textboxes de producto y precio
            TextoProducto.IsEnabled = false;
            TextoPrecio.IsEnabled = false;
            //se vuelve a habilitar el primer stackpanel
            AccionesStackPanel.IsEnabled = true;
            //y se inhabilita el segundo stackpanel (confirmar/cancelar)
            ConfirmStackPanel.IsEnabled = false;

        }
    }
}
