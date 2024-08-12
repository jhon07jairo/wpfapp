using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Security.Permissions;
using System.Data;
using System.CodeDom;

namespace AppCrud
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadGrid();
            CargarPaises();
        }

        SqlConnection con = new SqlConnection(@"Data Source=JHONEQUIPO\SQLEXPRESS;Initial Catalog=Personas;Integrated Security=True");

        public void LoadGrid()
        {
            SqlCommand cmd = new SqlCommand("select * from Persona", con);
            DataTable dt = new DataTable();
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();
            datagrid.ItemsSource = dt.DefaultView;
        }

        public void clearData() {
            txtNombre.Clear();
            txtApellido.Clear();
            txtDocumento.Clear();
            txtId.Clear();
            cmbPais.SelectedIndex = -1;
            cmbSexo.SelectedIndex = -1;
            datepicker.SelectedDate = null;
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            clearData();
        }

        private void CargarPaises()
        {
            string query = "SELECT CodigoPais, NombrePais FROM Pais";
            SqlCommand cmd = new SqlCommand(query, con);
            DataTable dtPaises = new DataTable();

            try
            {
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dtPaises);
                cmbPais.ItemsSource = dtPaises.DefaultView;
                cmbPais.DisplayMemberPath = "NombrePais"; // Nombre visible en el ComboBox
                cmbPais.SelectedValuePath = "CodigoPais"; // Valor seleccionado (CodigoPais)
            }
            finally
            {
                con.Close();
            }
        }


        public bool isValid()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtDocumento.Text))
            {
                MessageBox.Show("El documento es obligatorio", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (datepicker.SelectedDate == null)
            {
                MessageBox.Show("El país es obligatorio", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (cmbSexo.SelectedIndex == -1)
            {
                MessageBox.Show("El escoger tipo de sexo es obligatorio", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (cmbPais.SelectedIndex == -1)
            {
                MessageBox.Show("El país es obligatorio", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isValid())
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Persona VALUES(@Nombre, @Apellidos, @Documento, @FechaNacimiento, @Sexo, @CodigoPais)", con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text);
                    cmd.Parameters.AddWithValue("@Apellidos", txtApellido.Text);
                    cmd.Parameters.AddWithValue("@Documento", txtDocumento.Text);
                    cmd.Parameters.AddWithValue("@FechaNacimiento", datepicker.SelectedDate.HasValue ? datepicker.SelectedDate.Value : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Sexo", cmbSexo.SelectedValue.ToString());
                    cmd.Parameters.AddWithValue("@CodigoPais", cmbPais.SelectedValue.ToString());
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    LoadGrid();
                    MessageBox.Show("Guardado exitosamente", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                    clearData();
                }

            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("delete from Persona where ID = " + txtId.Text + " ", con);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Registro ha sido eliminado", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                con.Close();
                clearData();
                LoadGrid();
                con.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("No eliminado" + ex.Message);
            }
            finally 
            {
                con.Close();
            }     
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("UPDATE Persona SET Nombre = @Nombre, Apellidos = @Apellidos, Documento = @Documento, FechaNacimiento = @FechaNacimiento, Sexo = @Sexo, CodigoPais = @CodigoPais WHERE ID = @Id", con);

            // Agregamos los parámetros
            cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text);
            cmd.Parameters.AddWithValue("@Apellidos", txtApellido.Text);
            cmd.Parameters.AddWithValue("@Documento", txtDocumento.Text);

            // Para FechaNacimiento, verificamos si el DatePicker tiene un valor seleccionado
            if (datepicker.SelectedDate.HasValue)
            {
                cmd.Parameters.AddWithValue("@FechaNacimiento", datepicker.SelectedDate.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@FechaNacimiento", DBNull.Value);
            }

            // Para Sexo y Código de País, verificamos si hay un valor seleccionado
            cmd.Parameters.AddWithValue("@Sexo", cmbSexo.SelectedValue != null ? cmbSexo.SelectedValue.ToString() : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CodigoPais", cmbPais.SelectedValue != null ? cmbPais.SelectedValue.ToString() : (object)DBNull.Value);

            // Agregamos el parámetro para el ID
            cmd.Parameters.AddWithValue("@Id", txtId.Text);

            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Registro ha sido editado", "Updated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
                clearData();
                LoadGrid();
            }
        }

        private void datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (datagrid.SelectedItem != null)
            {
                txtId.Text = ((DataRowView)datagrid.SelectedItem)[0].ToString();
                txtNombre.Text = ((DataRowView)datagrid.SelectedItem)[1].ToString();
                txtApellido.Text = ((DataRowView)datagrid.SelectedItem)[2].ToString();
                txtDocumento.Text = ((DataRowView)datagrid.SelectedItem)[3].ToString();

                // Convertir el valor de la fecha a DateTime?, manejando nulos
                var fechaNacimiento = ((DataRowView)datagrid.SelectedItem)[4] as DateTime?;
                if (fechaNacimiento.HasValue)
                {
                    datepicker.SelectedDate = fechaNacimiento.Value;
                }
                else
                {
                    datepicker.SelectedDate = null; // o alguna otra acción si la fecha es nula
                }

                cmbSexo.SelectedValue = ((DataRowView)datagrid.SelectedItem)[5].ToString();
                cmbPais.SelectedValue = ((DataRowView)datagrid.SelectedItem)[6].ToString();
            }
        }



    }
}
