using System;
using System.Data;
using System.Data.Common;

public abstract class ConexaoBase<T, U> where T : DbConnection, new() where U : DbDataAdapter, new()
{
    //Propriedades
    public String Caminho { get; set; }
    public T Conexao { get; set; }
    public U DataAdapter { get; set; }

    #region Construtores
    public ConexaoBase()
    {
        Conexao = new T();
        DataAdapter = new U();
    }

    public ConexaoBase(String caminho)
    {
        Conexao = new T();
        DataAdapter = new U();
        Caminho = caminho;
    }

    #endregion

    public void ExecutaScript(string Script)
    {
        try
        {
            using (IDbConnection conn = new T())
            {
                conn.ConnectionString = Caminho;
                conn.Open();
                string sql = Script;
                using (IDbCommand objCommand = conn.CreateCommand())
                {
                    objCommand.CommandText = sql;
                    objCommand.ExecuteNonQuery();
                }
            }
        }
        catch (Exception erro)
        {
            throw new Exception(erro.Message, erro);
        }
    }

    public void ExecutaScript(string Script, int numeroParametros, object[] parametros, object[] valorParametros)
    {
        try
        {
            Conexao.ConnectionString = Caminho;
            IDbConnection conn = Conexao;

            conn.Open();
            string sql = Script;
            using (IDbCommand objCommand = conn.CreateCommand())
            {
                objCommand.CommandText = sql;
                for (int i = 0; i < numeroParametros; i++)
                {
                    var parameter = objCommand.CreateParameter();
                    parameter.ParameterName = "@" + parametros[i];
                    parameter.Value = valorParametros[i];

                    objCommand.Parameters.Add(parameter);
                }

                objCommand.ExecuteNonQuery();
            }
            conn.Close();

        }
        catch (Exception erro)
        {
            throw new Exception(erro.Message, erro);
        }
    }

    public DataTable ExecutaScriptRetornaDataTable(string Script)
    {
        try
        {
            Conexao.ConnectionString = Caminho;
            IDbConnection conn = Conexao;
            conn.Open();
            string sql = Script;
            using (IDbCommand objCommand = conn.CreateCommand())
            {
                objCommand.CommandText = sql;
                DataAdapter.SelectCommand = (DbCommand)objCommand;
                DataTable dtResultado = new DataTable();
                DataAdapter.Fill(dtResultado);
                conn.Close();
                return dtResultado;
            }
        }
        catch (Exception erro)
        {
            throw new Exception(erro.Message, erro);
        }
    }

    public DataTable ExecutaScriptRetornaDataTable(string conexao, string Script, int numeroParametros, object[] parametros, object[] valorParametros)
    {
        try
        {
            using (IDbConnection conn = new T())
            {
                conn.Open();
                string sql = Script;
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    for (int i = 0; i < numeroParametros; i++)
                    {
                        var parameter = cmd.CreateParameter();
                        parameter.ParameterName = "@" + parametros[i];
                        parameter.Value = valorParametros[i];

                        cmd.Parameters.Add(parameter);
                    }
                    using (DbDataAdapter dataAdapater = new U())
                    {
                        dataAdapater.SelectCommand = (DbCommand)cmd;
                        //dataAdapater.SelectCommand = cmd;
                        DataTable dtResultado = new DataTable();
                        dataAdapater.Fill(dtResultado);
                        return dtResultado;
                    }
                }
            }
        }
        catch (Exception erro)
        {
            throw new Exception(erro.Message, erro);
        }
    }
}